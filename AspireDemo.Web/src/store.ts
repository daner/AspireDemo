import {configureStore} from '@reduxjs/toolkit'
import { useDispatch } from 'react-redux'
import messageReducer from './reducers/messageReducer'

const store = configureStore({
    reducer: {
        chat: messageReducer
    }
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export default store
