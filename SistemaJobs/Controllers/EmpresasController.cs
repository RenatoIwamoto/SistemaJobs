using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SistemaJobs;
using SistemaJobs.ViewModel;

namespace SistemaJobs.Controllers
{
    public class EmpresasController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Empresas
        [Authorize]
        public ActionResult Index()
        {
            return this.RedirectToAction("Index", "Home");
            //return View();
        }

        // GET: Empresas/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            id = Convert.ToInt32(Session["usuarioLogadoID"]);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }

            ViewBag.Imagem = empresa.Imagem;
            empresa.CNPJ = Convert.ToUInt64(empresa.CNPJ).ToString(@"00\.000\.000\/0000\-00");
            empresa.Telefone = Convert.ToUInt64(empresa.Telefone).ToString(@"\(00\)00000\-0000");

            return View(empresa);
        }

        // GET: Empresas/Create
        public ActionResult Create()
        {
            PopularDdlEstado();
            return View();
        }

        // POST: Empresas/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdEmpresa,Nome,CNPJ,Telefone,Email,Estado,Cidade,Usuario,Senha")] Empresa empresa)
        {
            ValidarUnicidade(empresa);

            if (ModelState.IsValid)
            {
                db.Empresa.Add(empresa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopularDdlEstado();
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }

            ViewBag.ImagemSalva = empresa.Imagem;
            ViewBag.Usuario = empresa.Usuario;
            ViewBag.Senha = empresa.Senha;
            PopularDdlEstado();

            return View(empresa);
        }

        //Popula a lista de Estados brasileiros
        protected void PopularDdlEstado()
        {
            List<SelectListItem> ddlEstadoItems = new List<SelectListItem>();
            ddlEstadoItems.Add(new SelectListItem { Value = "AC", Text = "AC" });
            ddlEstadoItems.Add(new SelectListItem { Value = "AL", Text = "AL" });
            ddlEstadoItems.Add(new SelectListItem { Value = "AP", Text = "AP" });
            ddlEstadoItems.Add(new SelectListItem { Value = "AM", Text = "AM" });
            ddlEstadoItems.Add(new SelectListItem { Value = "BA", Text = "BA" });
            ddlEstadoItems.Add(new SelectListItem { Value = "CE", Text = "CE" });
            ddlEstadoItems.Add(new SelectListItem { Value = "DF", Text = "DF" });
            ddlEstadoItems.Add(new SelectListItem { Value = "ES", Text = "ES" });
            ddlEstadoItems.Add(new SelectListItem { Value = "GO", Text = "GO" });
            ddlEstadoItems.Add(new SelectListItem { Value = "MA", Text = "MA" });
            ddlEstadoItems.Add(new SelectListItem { Value = "MT", Text = "MT" });
            ddlEstadoItems.Add(new SelectListItem { Value = "MS", Text = "MS" });
            ddlEstadoItems.Add(new SelectListItem { Value = "MG", Text = "MG" });
            ddlEstadoItems.Add(new SelectListItem { Value = "PA", Text = "PA" });
            ddlEstadoItems.Add(new SelectListItem { Value = "PB", Text = "PB" });
            ddlEstadoItems.Add(new SelectListItem { Value = "PR", Text = "PR" });
            ddlEstadoItems.Add(new SelectListItem { Value = "PE", Text = "PE" });
            ddlEstadoItems.Add(new SelectListItem { Value = "PI", Text = "PI" });
            ddlEstadoItems.Add(new SelectListItem { Value = "RJ", Text = "RJ" });
            ddlEstadoItems.Add(new SelectListItem { Value = "RN", Text = "RN" });
            ddlEstadoItems.Add(new SelectListItem { Value = "RS", Text = "RS" });
            ddlEstadoItems.Add(new SelectListItem { Value = "RO", Text = "RO" });
            ddlEstadoItems.Add(new SelectListItem { Value = "RR", Text = "RR" });
            ddlEstadoItems.Add(new SelectListItem { Value = "SC", Text = "SC" });
            ddlEstadoItems.Add(new SelectListItem { Value = "SP", Text = "SP" });
            ddlEstadoItems.Add(new SelectListItem { Value = "SE", Text = "SE" });
            ddlEstadoItems.Add(new SelectListItem { Value = "TO", Text = "TO" });

            ViewBag.Estado = ddlEstadoItems;
        }

        // Verifica se o campo é único
        public ActionResult ValidarUnicidade(Empresa empresa)
        {
            string pattern = @"[^0-9]";
            Regex rgx = new Regex(pattern);

            if (empresa.CNPJ != null)
            {
                empresa.CNPJ = rgx.Replace(empresa.CNPJ, "");
            }

            if (empresa.Telefone != null)
            {
                empresa.Telefone = rgx.Replace(empresa.Telefone, "");
            }

            var cnpj = db.Empresa.Where(u => u.CNPJ == empresa.CNPJ).Count() == 0;
            var telefone = db.Empresa.Where(u => u.Telefone == empresa.Telefone).Count() == 0;
            var email = db.Empresa.Where(u => u.Email == empresa.Email).Count() == 0;
            var user = db.Empresa.Where(u => u.Usuario == empresa.Usuario).Count() == 0;
            var senha = db.Empresa.Where(u => u.Senha == empresa.Senha).Count() == 0;

            var telefone2 = db.Funcionario.Where(u => u.Telefone == empresa.Telefone).Count() == 0;
            var email2 = db.Funcionario.Where(u => u.Email == empresa.Email).Count() == 0;
            var user2 = db.Funcionario.Where(u => u.Usuario == empresa.Usuario).Count() == 0;
            var senha2 = db.Funcionario.Where(u => u.Senha == empresa.Senha).Count() == 0;

            if (cnpj && telefone && email && user && senha && telefone2 && email2 && user2 && senha2)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else if (cnpj == false)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else if (telefone == false || telefone2 == false)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else if (email == false || email2 == false)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else if (user == false || user2 == false)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else if (senha == false || senha2 == false)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Empresas/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdEmpresa,Nome,CNPJ,Telefone,Email,Estado,Cidade,Usuario,Senha,Sobre")] Empresa empresa, string ImagemSalva)
        {
            bool imgValidada = false;
            HttpPostedFileBase imagem = Request.Files[0];

            if (imagem.FileName != "")
            {
                imgValidada = ValidaImagem(imagem);

                if (imgValidada)
                {
                    //Salva o arquivo
                    if (imagem.ContentLength > 0)
                    {
                        var uploadPath = Server.MapPath("~/Images");
                        string caminhoArquivo = Path.Combine(@uploadPath, Path.GetFileName(imagem.FileName));

                        imagem.SaveAs(caminhoArquivo);
                    }
                }
                else
                {
                    return View(empresa);
                }
            }

            empresa.Imagem = imgValidada && imagem.FileName != "" ? imagem.FileName : ImagemSalva;

            if (ModelState.IsValid)
            {
                db.Entry(empresa).State = EntityState.Modified;

                string pattern = @"[^0-9]";
                Regex rgx = new Regex(pattern);

                if (empresa.CNPJ != null)
                    empresa.CNPJ = rgx.Replace(empresa.CNPJ, "");

                if (empresa.Telefone != null)
                    empresa.Telefone = rgx.Replace(empresa.Telefone, "");

                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(empresa);
        }

        public bool ValidaImagem(HttpPostedFileBase imagem)
        {
            var supportedTypes = new[] { "jpeg", "jpg", "gif", "png" };
            var imagemExt = Path.GetExtension(imagem.FileName).Substring(1);

            if (imagem == null || imagem.ContentLength == 0)
            {
                ModelState.AddModelError("imagem", "Este campo é obrigatório");
                return false;
            }
            else if (!supportedTypes.Contains(imagemExt))
            {
                ModelState.AddModelError("imagem", "Escolha uma imagem GIF, JPG ou PNG.");
                return false;
            }
            //else if (imagem.ContentLength > 1024)
            //{
            //    ModelState.AddModelError("imagem", "A imagem deve ter no máximo 1mb.");
            //    return false;
            //}

            return true;
        }

        // GET: Empresas/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresa.Find(id);
            db.Empresa.Remove(empresa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
