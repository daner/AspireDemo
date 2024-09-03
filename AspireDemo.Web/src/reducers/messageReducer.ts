import {createSlice, PayloadAction} from '@reduxjs/toolkit'
import {Message} from "../models/Message";
import {AppDispatch} from "../store";
import messageService from "../services/messageService";

const messageSlice = createSlice({
    name: 'message',
    initialState: {room: "", messages: []} as { room: string, messages: Message[] },
    reducers: {
        pushMessage(state, action: PayloadAction<Message>) {
            if(state.room === action.payload.room) {
                return {...state, messages: [action.payload, ...state.messages]}    
            }
            return state;
        },
        setMessages(state, action: PayloadAction<Message[]>) {
            return {...state, messages: action.payload}
        },
        setRoom(state, action: PayloadAction<string>) {
            return {...state, room: action.payload}
        }
    }
})

export const {pushMessage, setMessages, setRoom} = messageSlice.actions

export const joinRoom = (room: string) => {
    return async (dispatch: AppDispatch) => {
        dispatch(setMessages([]));
        const messageResult = await messageService.getAllForRoom(room);
        if (messageResult.ok) {
            dispatch(setRoom(room));
            dispatch(setMessages(messageResult.value));
        } else {
            //Handle error
        }
    }
}

export const leaveRoom = () => {
    return async (dispatch: AppDispatch) => {
        dispatch(setRoom(""));
        dispatch(setMessages([]));
    }
}

export default messageSlice.reducer