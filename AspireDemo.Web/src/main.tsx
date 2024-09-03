import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Provider } from 'react-redux'
import { HubConnectionContextProvider } from './contexts/HubConnectionContext'
import App from './App'
import './index.css'
import store from './store'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Provider store={store}>
            <HubConnectionContextProvider>
                <App />
            </HubConnectionContextProvider>
        </Provider>
    </StrictMode>,
)
