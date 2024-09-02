import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import userService from '../services/userService.ts'
import {AppDispatch} from "../store.ts";
import {AuthorizedStatus} from "../models/AuthorizedStatus.ts";

const userSlice = createSlice({
    name: 'user',
    initialState: { authorized: false, claims: [] } as AuthorizedStatus,
    reducers: {
        setAuthorized: (_, action: PayloadAction<AuthorizedStatus>) => {
            return action.payload;  
        }
    }
})

export const checkAuthorizedStatus = () => {
    return async (dispatch: AppDispatch) => {
        const result = await userService.getAuthorizedStatus()
        dispatch(setAuthorized(result));
    }
}

export const {setAuthorized} = userSlice.actions;
export default userSlice.reducer;