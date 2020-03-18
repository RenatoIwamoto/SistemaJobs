using SistemaJobs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaJobs.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            //return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(VmFuncionarioEmpresa model)
        {
            VmFuncionarioEmpresa vm = new VmFuncionarioEmpresa();

            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            // esta action trata o post (login)
            if (ModelState.IsValid) //verifica se é válido
            {
                using (HiredItEntities db = new HiredItEntities())
                {
                    var funcLogin = db.Funcionario.FirstOrDefault(f => f.Usuario == model.Usuario && f.Senha == model.Senha);
                    var empLogin = db.Empresa.FirstOrDefault(e => e.Usuario.Equals(model.Usuario) && e.Senha.Equals(model.Senha));

                    if (funcLogin != null)
                    {
                        vm.Usuario = funcLogin.Usuario;
                        vm.Senha = funcLogin.Senha;
                    }
                    if (empLogin != null)
                    {
                        vm.Usuario = empLogin.Usuario;
                        vm.Senha = empLogin.Senha;
                    }
                    
                    if (vm.Usuario != null || vm.Senha != null)
                    {
                        if (funcLogin != null)
                        {
                            Session["usuarioLogadoID"] = funcLogin.IdFuncionario.ToString();
                            Session["tipoUsuario"] = "funcionario";
                            FormsAuthentication.SetAuthCookie(funcLogin.Usuario.ToString(), false);
                        }
                        if (empLogin != null)
                        {
                            Session["usuarioLogadoID"] = empLogin.IdEmpresa.ToString();
                            Session["tipoUsuario"] = "empresa";
                            FormsAuthentication.SetAuthCookie(empLogin.Usuario.ToString(), false);
                        }

                        db.Dispose();

                        if (funcLogin != null)
                            return RedirectToAction("Details", "Funcionarios");

                        return RedirectToAction("Details", "Empresas");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Usuário ou Senha está incorreto!";
                    }
                }
            }
            return View(model);
        }

        // GET: Login
        //public ActionResult Index()
        //{
        //    VmFuncionarioEmpresa vm = new VmFuncionarioEmpresa();

        //    var funcLogin = db.Funcionario.FirstOrDefault(e => e.Usuario == "renat@email.com");
        //    var empLogin = db.Empresa.FirstOrDefault(e => e.Usuario == "usuario");

        //    if (funcLogin != null)
        //    {
        //        vm.Funcionario = funcLogin;
        //    }
        //    if (empLogin != null)
        //    {
        //        vm.Empresa = empLogin;
        //    }

        //    ViewBag.Usuario = vm;

        //    //foreach (var item in vm.Empresa)
        //    //{
        //    //    var teste = item.
        //    //}

        //    return View();
        //}
    }
}