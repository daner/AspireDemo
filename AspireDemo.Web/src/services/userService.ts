import axios from 'axios'
import {AuthorizedStatus} from "../models/AuthorizedStatus";

const baseUrl = "/auth"

const getAuthorizedStatus = async (): Promise<AuthorizedStatus> => {
    try {
        const response = await axios.get<AuthorizedStatus>(baseUrl + "/me")
        return response.data
    } catch (e) {
        return { isAuthenticated: false, claims: []}
    }
}

export default {getAuthorizedStatus}