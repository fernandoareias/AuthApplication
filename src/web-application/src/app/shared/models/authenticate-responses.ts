export interface UserLoginView {
  accessToken: string,
  expiresIn: number,
  userToken: UserTokenView
}


export interface UserTokenView {
  id: string,
  email: string,
  claims: Array<UsuarioClaimView>[]
}

export interface UsuarioClaimView {
  value: string,
  type: string
}
