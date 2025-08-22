public interface IContainer {
    void OnInit();

    void OnServerCommand(ServerBase server, BasePackage package);
    
    void OnClientCommand(ServerBase server, BasePackage package);
}