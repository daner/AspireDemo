import {useState, createContext, useEffect, PropsWithChildren, useContext, useRef} from 'react'
import {HubConnectionBuilder, HubConnection} from '@microsoft/signalr'
import store, {useAppDispatch} from "../store.ts";
import {pushMessage} from "../reducers/messageReducer.ts";
import {Message} from "../models/Message.ts";

export type HubConnectionContextValue = {
    connection: HubConnection | null
}

const HubConnectionContext = createContext<HubConnectionContextValue>({connection: null})

export const useHubConnectionContext = () => useContext(HubConnectionContext)

export const HubConnectionContextProvider = (props: PropsWithChildren<any>) => {

    let room = "";
    
    store.subscribe(() => {
        room = store.getState().chat.room;
    });
    
    const dispatch = useAppDispatch();
    
    const [connection, setConnection] = useState<HubConnection | null>(null)

    const initialized = useRef(false)
    
    useEffect(() => {

        if (initialized.current) {
            return
        } 
        
        initialized.current = true
        
        const connection = new HubConnectionBuilder()
            .withUrl("/api/notifications")
            .withAutomaticReconnect()
            .build();

        connection.start().then(() => {
            setConnection(connection)
        })
        
        connection.onclose(() => {
            console.log("Connection closed")
            initialized.current = false
            setConnection(null)
        })
        
        connection.onreconnected(() => {
            console.log("Reconnected")
            if(room.length > 0) {
                connection.invoke("JoinRoom", room)
            }
        })
        
        connection.on("ChatMessage", (message: Message) => {
            if(room === message.room) {
                dispatch(pushMessage(message))    
            }
            else {
                console.error("Got message for wrong room")
            }
        })
    }, [])

    return (
        <HubConnectionContext.Provider value={{connection}}>
            {props.children}
        </HubConnectionContext.Provider>
    )
}

export default HubConnectionContext