import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import userService from '../services/userService'
import {AppDispatch} from "../store";
import {AuthorizedStatus} from "../models/AuthorizedStatus";

const userSlice = createSlice({
    name: 'user',
    initialState: { isAuthenticated: false, claims: [] } as AuthorizedStatus,
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