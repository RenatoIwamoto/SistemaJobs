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

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(VmFuncionarioEmpresa model)
        {
            VmFuncionarioEmpresa vm = new VmFuncionarioEmpresa();

            // esta action trata o post (login)
            if (ModelState.IsValid) //verifica se é válido
            {
                using (HiredItEntities db = new HiredItEntities())
                {
                    var funcLogin = db.Funcionario.FirstOrDefault(e => e.Usuario == model.Funcionario.Usuario && e.Senha == model.Funcionario.Senha);
                    var empLogin = db.Empresa.FirstOrDefault(e => e.Usuario.Equals(model.Empresa.Usuario) && e.Senha.Equals(model.Empresa.Senha));

                    if (funcLogin != null)
                    {
                        vm.Funcionario = funcLogin;
                    }
                    if (empLogin != null)
                    {
                        vm.Empresa = empLogin;
                    }
                    
                    if (vm.Funcionario != null || vm.Empresa != null)
                    {
                        if (vm.Funcionario != null)
                        {
                            Session["usuarioLogadoID"] = vm.Funcionario.IdFuncionario.ToString();
                            Session["tipoUsuario"] = "funcionario";
                            FormsAuthentication.SetAuthCookie(vm.Funcionario.Usuario.ToString(), false);
                        }
                        if (vm.Empresa != null)
                        {
                            Session["usuarioLogadoID"] = vm.Empresa.IdEmpresa.ToString();
                            Session["tipoUsuario"] = "empresa";
                            FormsAuthentication.SetAuthCookie(vm.Empresa.Usuario.ToString(), false);
                        }

                        db.Dispose();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "User Name ou Senha está incorreto!";
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