using AuthApplication.Identity.API.Configurations;
using AuthApplication.Identity.API.NovaPasta;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace AuthApplication.Identity.API.Controllers
{
    [Route("api/account")]
    public class AuthController : MainController
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AuthController> _log;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings,
            ILogger<AuthController> log
        ) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _log = log;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _log.LogInformation("User logged out.");

                return Ok("User logged out.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw ex;
            }
        }



        [HttpPost("register")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<IActionResult> Registrar(UserRegister usuarioRegistro)
        {
            try
            {
                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                var user = new IdentityUser()
                {
                    UserName = usuarioRegistro.Email,
                    Email = usuarioRegistro.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, usuarioRegistro.Password);

                if (result.Succeeded)
                {
                    return CustomResponse(await GerarJwt(usuarioRegistro.Email));
                }

                foreach (var erro in result.Errors)
                    AdicionarErroProcessamento(erro.Description);

                return CustomResponse();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw ex;
            }
        }


        [HttpPost("login")]
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Login(UserLogin usuarioLogin)
        {
            try
            {


                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Password, false, true);

                if (result.Succeeded)
                    return CustomResponse(await GerarJwt(usuarioLogin.Email));


                if (result.IsLockedOut)
                {
                    AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas.");
                    return CustomResponse();
                }

                AdicionarErroProcessamento("Usuário ou senha incorretos.");
                return CustomResponse();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw ex;
            }
        }

        

        private async Task<UserLoginView> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await ObterClaimsUsuario(claims, user);
            var encodedToken = CodificarToken(identityClaims);

            return ObterRespostaToken(encodedToken, user, claims);
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {

            var roles = await _userManager.GetRolesAsync(user);


            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id)); // Issuer
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // (JWT ID)
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixExpochDate(DateTime.Now).ToString())); // Not Before
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixExpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64)); // Issued At
            
            foreach (var role in roles)
                claims.Add(new Claim(type: "role", value: role));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return await Task.FromResult(identityClaims);
        }

        private string CodificarToken(ClaimsIdentity identityClaims)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            {
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.ValidOn,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private UserLoginView ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            return new UserLoginView()
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpirationHours).TotalSeconds,
                UserToken = new UserTokenView()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaimView() { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixExpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(year: 1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
