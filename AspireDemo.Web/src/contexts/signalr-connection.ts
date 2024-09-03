import * as signalR from '@microsoft/signalr';

class Connector {
    private readonly connection: signalR.HubConnection;
    static instance: Connector;
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/api/notifications")
            .withAutomaticReconnect()
            .build();
        this.connection.start().catch(err => console.log(err));
    }

    public static getInstance(): Connector {
        if (!Connector.instance)
            Connector.instance = new Connector();
        return Connector.instance;
    }
    
    public getConnection(): signalR.HubConnection {
        return this.connection;
    }
}

export default Connector.getInstance;