import {useState, createContext, useEffect, PropsWithChildren, useContext} from 'react'
import {HubConnection} from '@microsoft/signalr'
import {useAppDispatch} from "../store.ts";
import {leaveRoom, pushMessage} from "../reducers/messageReducer.ts";
import {Message} from "../models/Message.ts";
import connector from './signalr-connection.ts'

export type HubConnectionContextValue = {
    connection?: HubConnection
}

const HubConnectionContext = createContext<HubConnectionContextValue>({})

export const useHubConnectionContext = () => useContext(HubConnectionContext)

export const HubConnectionContextProvider = (props: PropsWithChildren<any>) => {

    const dispatch = useAppDispatch();
    const [connectionRef, setConnectionRef] = useState<HubConnection>()

    useEffect(() => {
        setConnectionRef(connector().getConnection());
    }, []);

    useEffect(() => {

        if (connectionRef === undefined) return

        connectionRef.onclose(() => {
            dispatch(leaveRoom())
        })

        connectionRef.onreconnected(() => {
            console.log("Reconnected")
        })

        connectionRef?.on("ChatMessage", (message: Message) => {
            dispatch(pushMessage(message))
        })
    }, [connectionRef])

    return (
        <HubConnectionContext.Provider value={{connection: connectionRef}}>
            {props.children}
        </HubConnectionContext.Provider>
    )
}

export default HubConnectionContext