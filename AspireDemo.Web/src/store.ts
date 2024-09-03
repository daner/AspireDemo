﻿import { configureStore } from '@reduxjs/toolkit'
import { useDispatch } from 'react-redux'
import messageReducer from './reducers/messageReducer'
import userReducer from "./reducers/userReducer";

const store = configureStore({
    reducer: {
        chat: messageReducer,
        user: userReducer
    }
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export default store
