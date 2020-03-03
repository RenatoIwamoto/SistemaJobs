using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaJobs;

namespace SistemaJobs.Controllers
{
    public class FuncionariosController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Funcionarios
        public ActionResult Index()
        {
            return this.RedirectToAction("Index", "Home");
            //return View(db.Funcionario.ToList());
        }

        // GET: Funcionarios/Details/5
        public ActionResult Details(int? id)
        {
            id = 1;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcionario funcionario = db.Funcionario.Find(id);
            if (funcionario == null)
            {
                return HttpNotFound();
            }

            ViewBag.Imagem = funcionario.Imagem;

            return View(funcionario);
        }

        // GET: Funcionarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Funcionarios/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFuncionario,Nome,Sobrenome,CPF,Telefone,Email,Estado,Cidade,Usuario,Senha")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Funcionario.Add(funcionario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funcionario);
        }

        // GET: Funcionarios/Edit/5
        public ActionResult Edit(int? id)
        {
            id = 1;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Funcionario funcionario = db.Funcionario.Find(id);

            if (funcionario == null)
            {
                return HttpNotFound();
            }

            ViewBag.ImagemSalva = funcionario.Imagem;
            ViewBag.Usuario = funcionario.Usuario;
            ViewBag.Senha = funcionario.Senha;
            PopularDdlEstado();

            return View(funcionario);
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

        // POST: Funcionarios/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFuncionario,Nome,Sobrenome,CPF,Telefone,Email,Estado,Cidade,Usuario,Senha,Imagem,Qualificacoes,Experiencia,Sobre")] Funcionario funcionario, string ImagemSalva)
        {
            funcionario.IdFuncionario = 1;

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
                    return View(funcionario);
                }
            }

            funcionario.Imagem = imgValidada && imagem.FileName != "" ? imagem.FileName : ImagemSalva;

            if (ModelState.IsValid)
            {
                db.Entry(funcionario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }

            PopularDdlEstado();

            return View(funcionario);
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

        // GET: Funcionarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcionario funcionario = db.Funcionario.Find(id);
            if (funcionario == null)
            {
                return HttpNotFound();
            }
            return View(funcionario);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Funcionario funcionario = db.Funcionario.Find(id);
            db.Funcionario.Remove(funcionario);
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
