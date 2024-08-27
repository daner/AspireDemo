import axios from 'axios'
import {Message} from "../models/Message.ts";
import {Result} from "../models/Result.ts";

const baseUrl = "/api/message"

const getAllForRoom = async (room: string): Promise<Result<Message[], any>> => {
    try {
        const response = await axios.get<Message[]>(baseUrl + "/" + room)
        return {ok: true, value: response.data}
    } catch (e) {
        return {ok: false, error: e}
    }
}

const sendMessage = async (room: string, text: string): Promise<Result<Message, any>> => {
    try {
        const body = {text}
        const response = await axios.post<Message>(baseUrl + "/" + room, body)
        return {ok: true, value: response.data}
    } catch (e) {
        return {ok: false, error: e}
    }
}

export default {getAllForRoom, sendMessage}