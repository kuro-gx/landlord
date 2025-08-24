using System;
using System.Threading;
using Server.Controller;

namespace Server {
    public class Program {
        static void Main(string[] args) {
            NetServer server = new NetServer();
            server.StartServer(NetDefine.IP, NetDefine.ServerPort);

            LoginController loginController = new LoginController();
            server.RegisterCommand(NetDefine.CMD_RegisterCode, loginController);

            while (true) {
                Thread.Sleep(1);
            }
        }
    }
}