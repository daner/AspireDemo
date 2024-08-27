import Button from "../components/Button.tsx";
import {useState} from 'react'

interface Message {
    date: Date,
    sender: string;
    text: string
}

const Messages = () => {
    const [roomInput, setRoomInput] = useState<string>("")
    const [messageInput, setMessageInput] = useState<string>("")

    const [room, setRoom] = useState<string | null>(null)
    const [messages, setMessages] = useState<Message[]>([])

    const inputClasses = "block rounded-md border-0 py-1.5 sm:text-sm sm:leading-6 focus:ring-2 focus:ring-inset ring-1 ring-inset shadow-sm text-gray-900 ring-gray-300 placeholder:text-gray-400 focus:ring-indigo-600"

    const joinRoom = () => {
        if (roomInput.length > 0) {
            setRoom(roomInput)
        }
    }

    const leaveRoom = () => {
        setRoom(null)
    }
    
    const addMessage = () => {
        if (messageInput.length > 0) {
            setMessages([{date: new Date(), sender: "User", text: messageInput}, ...messages])
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
                            room === null ? (
                                <div className="flex row gap-4">
                                    <input id="room" className={inputClasses} value={roomInput}
                                           onChange={(event) => setRoomInput(event.target.value)}/>
                                    <Button click={joinRoom}>Join room</Button>
                                </div>
                            ) : (
                                <div className="flex row gap-4">
                                    <div className="text-xl py-1">{room}</div>
                                    <input id="message" className={inputClasses} value={messageInput}
                                           onChange={(event) => setMessageInput(event.target.value)}/>
                                    <Button click={addMessage}>Send</Button>
                                    <Button click={leaveRoom}>Leave room</Button>
                                </div>
                            )
                        }
                        <div className="flex-grow overflow-y-auto bg-gray-50">
                            <pre className="px-2 py-1">
                                {
                                    messages.map((message, i) => (
                                        <div key={i}>
                                            <span>[{message.date.toLocaleTimeString()}]</span>
                                            <span className="ml-2">{message.sender}:</span>
                                            <span className="ml-4">{message.text}</span>
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