using System.Threading;
using SqlSugar;

namespace Server {
    public class Program {
        static void Main(string[] args) {
            NetServer server = new NetServer();
            server.StartServer(NetDefine.IP, NetDefine.ServerPort);

            SqlSugarClient db = DBMgr.Instance.InitDB();

            UserController userController = new UserController(new UserService(db));
            MatchController matchController = new MatchController(new FightService(db));
            FightController fightController = new FightController(new FightService(db));
            // 注册指令集
            server.RegisterCommand(NetDefine.CMD_RegisterCode, userController);
            server.RegisterCommand(NetDefine.CMD_LoginCode, userController);
            server.RegisterCommand(NetDefine.CMD_UpdateUserInfoCode, userController);
            server.RegisterCommand(NetDefine.CMD_MatchCode, matchController);
            server.RegisterCommand(NetDefine.CMD_CallLordCode, fightController);
            server.RegisterCommand(NetDefine.CMD_GrabLordCode, fightController);
            server.RegisterCommand(NetDefine.CMD_RaiseCode, fightController);
            server.RegisterCommand(NetDefine.CMD_PlayHandCode, fightController);

            while (true) {
                Thread.Sleep(10);
            }
        }
    }
}