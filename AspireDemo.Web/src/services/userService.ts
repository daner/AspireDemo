import axios from 'axios'
import {AuthorizedStatus} from "../models/AuthorizedStatus.ts";

const baseUrl = "/auth"

const getAuthorizedStatus = async (): Promise<AuthorizedStatus> => {
    try {
        const response = await axios.get<AuthorizedStatus>(baseUrl + "/me")
        return response.data
    } catch (e) {
        return {authorized: false, claims: []}
    }
}

export default {getAuthorizedStatus}