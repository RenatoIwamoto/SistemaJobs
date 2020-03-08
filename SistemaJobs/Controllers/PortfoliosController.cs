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
    public class PortfoliosController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Portfolios
        public ActionResult Index()
        {
            //var portfolio = db.Portfolio.Include(p => p.Funcionario);
            return View(db.Portfolio.ToList());
        }

        // GET: Portfolios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Portfolio portfolio = db.Portfolio.Find(id);
            if (portfolio == null)
            {
                return HttpNotFound();
            }

            ViewBag.Imagem = portfolio.Imagem;
            return View(portfolio);
        }

        // GET: Portfolios/Create
        public ActionResult Create()
        {
            ViewBag.IdFuncionario = 16;
            //ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome");
            return View();
        }

        // POST: Portfolios/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPorfolio,Titulo,Descricao,Imagem,TempoTrabalho,IdFuncionario")] Portfolio portfolio)
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
                    return View(portfolio);
                }
            }

            portfolio.Imagem = imgValidada ? imagem.FileName : "";

            if (ModelState.IsValid)
            {
                db.Portfolio.Add(portfolio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", portfolio.IdFuncionario);
            return View(portfolio);
        }

        // GET: Portfolios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Portfolio portfolio = db.Portfolio.Find(id);
            if (portfolio == null)
            {
                return HttpNotFound();
            }

            //ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", portfolio.IdFuncionario);
            ViewBag.ImagemSalva = portfolio.Imagem;
            return View(portfolio);
        }

        // POST: Portfolios/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPorfolio,Titulo,Descricao,Imagem,TempoTrabalho,IdFuncionario")] Portfolio portfolio, string ImagemSalva)
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
                    return View(portfolio);
                }
            }

            portfolio.Imagem = imgValidada && imagem.FileName != null ? imagem.FileName : ImagemSalva;

            if (ModelState.IsValid)
            {
                db.Entry(portfolio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", portfolio.IdFuncionario);
            return View(portfolio);
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

        //GET: Portfolios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Portfolio portfolio = db.Portfolio.Find(id);
            if (portfolio == null)
            {
                return HttpNotFound();
            }
            return View(portfolio);
        }

        //POST: Portfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Portfolio portfolio = db.Portfolio.Find(id);
            db.Portfolio.Remove(portfolio);
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
