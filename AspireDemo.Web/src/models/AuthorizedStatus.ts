export interface Claim {
    type: string
    value: string
}

export interface AuthorizedStatus {
    isAuthenticated: boolean
    claims: Claim[]  
}