import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {Provider} from 'react-redux'
import {HubConnectionContextProvider} from './contexts/HubConnectionContext.tsx'
import App from './App.tsx'
import './index.css'
import store from './store'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Provider store={store}>
            <HubConnectionContextProvider>
                <App/>
            </HubConnectionContextProvider>
        </Provider>
    </StrictMode>,
)
