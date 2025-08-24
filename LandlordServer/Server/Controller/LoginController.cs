using System;

namespace Server.Controller {
    /// <summary>
    /// 登录控制器
    /// </summary>
    public class LoginController : IContainer {
        public void OnInit() {
        }

        public void OnServerCommand(ServerBase server, BasePackage package) {
            switch (package.Code) {
                case NetDefine.CMD_RegisterCode:
                    OnRegisterHandle(server, package);
                    break;
            }
        }

        public void OnClientCommand(ServerBase server, BasePackage package) {
        }

        /// <summary>
        /// 处理注册事件
        /// </summary>
        private void OnRegisterHandle(ServerBase server, BasePackage package) {
            RegisterReq req = RegisterReq.Parser.ParseFrom(package.Data);
            Console.WriteLine(req.ToString());
        }
    }
}