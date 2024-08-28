import Button from "../components/Button.tsx";
import {useState} from 'react'
import {Message} from "../models/Message.ts";
import {useSelector} from 'react-redux'
import {joinRoom, leaveRoom} from "../reducers/messageReducer.ts";
import {RootState, useAppDispatch} from "../store.ts";
import {useHubConnectionContext} from "../contexts/HubConnectionContext.tsx";
import messageService from "../services/messageService.ts";

const Messages = () => {

    const hubContext = useHubConnectionContext();
    
    const messages = useSelector<RootState, Message[]>((state) => {
        return state.chat.messages;
    })

    const room = useSelector<RootState, string>((state) => {
        return state.chat.room;
    })

    const dispatch = useAppDispatch()

    const [roomInput, setRoomInput] = useState<string>("")
    const [messageInput, setMessageInput] = useState<string>("")
    const inputClasses = "block rounded-md border-0 py-1.5 sm:text-sm sm:leading-6 focus:ring-2 focus:ring-inset ring-1 ring-inset shadow-sm text-gray-900 ring-gray-300 placeholder:text-gray-400 focus:ring-indigo-600"

    const handleJoinRoom = () => {
        if (roomInput.length > 0) {
            if(hubContext.connection != null) {
                hubContext.connection.invoke("JoinRoom", roomInput)
            }
            dispatch(joinRoom(roomInput))
        }
    }

    const handleLeaveRoom = () => {
        if(hubContext.connection != null) {
            hubContext.connection.invoke("LeaveRoom", room)
        }
        dispatch(leaveRoom())
    }

    const handleAddMessage = async () => {
        if (messageInput.length > 0 && room.length > 0) {
            await messageService.sendMessage(room, messageInput)
            setMessageInput("")
        }
    }

    return (
        <>
            <header>
                <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                    <h1 className="text-3xl font-bold leading-tight tracking-tight text-gray-900">Messages</h1>
                </div>
            </header>
            <main>
                <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
                    <div className="flex flex-col h-[calc(100vh-13rem)] justify-between gap-4">
                        {
                            room.length === 0 ? (
                                <div className="flex row gap-4">
                                    <input id="room" className={inputClasses} value={roomInput}
                                           onChange={(event) => setRoomInput(event.target.value)}/>
                                    <Button click={handleJoinRoom}>Join room</Button>
                                </div>
                            ) : (
                                <>
                                    <div className="text-xl py-1">{room}</div>
                                    <div className="flex row gap-4">
                                        <input id="message" className={inputClasses} value={messageInput}
                                               onChange={(event) => setMessageInput(event.target.value)}/>
                                        <Button click={handleAddMessage}>Send</Button>
                                        <Button click={handleLeaveRoom}>Leave room</Button>
                                    </div>
                                </>
                            )
                        }
                        <div className="flex-grow overflow-y-auto bg-gray-50">
                            <pre className="px-2 py-1 text-pretty">
                                {
                                    messages.map((message, i) => (
                                        <div key={i} className="mt-2">
                                            <span>[{message.timestamp}]</span>
                                            <span className="ml-2">{message.username}:</span>
                                            <span className="ml-2">{message.text}</span>
                                            <br/>
                                        </div>
                                    ))
                                }
                            </pre>
                        </div>
                    </div>
                </div>
            </main>
        </>
    )
}

export default Messages