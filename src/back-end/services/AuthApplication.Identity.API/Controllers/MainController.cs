using System.Collections.Generic;
using System.Linq;
using AuthApplication.Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;

namespace AuthApplication.Identity.API.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
     

        public ICollection<string> Erros = new List<string>();

     



        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(result);

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected bool ResponsePossuiErros(ResponseResult response)
        {
            if (response == null || !response.Errors.Mensagens.Any())
                return false;

            foreach (var mensagem in response.Errors.Mensagens)
                AdicionarErroProcessamento(mensagem);

            return true;
        }

        protected bool OperacaoValida()
            => !Erros.Any();

        protected void AdicionarErroProcessamento(string erro)
            => Erros.Add(erro);

        protected void LimparErroProcessamento()
            => Erros.Clear();
    }
}
