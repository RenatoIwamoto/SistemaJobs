using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaJobs;

namespace SistemaJobs.Controllers
{
    public class MensagensController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Mensagens
        public ActionResult Index()
        {
            return View(db.Mensagem.ToList());
        }

        // GET: Mensagens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagem.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // GET: Mensagens/Create
        public ActionResult Create()
        {
            var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var tipoUsuarioLogado = Session["tipoUsuario"].ToString();

            ViewBag.Imagem = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Imagem.ToString();
            ViewBag.Nome = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Nome.ToString();

            return View();
        }

        // POST: Mensagens/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMensagem,IdRemetente,IdDestinatario,Mensagem1,Data")] Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                db.Mensagem.Add(mensagem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mensagem);
        }

        // GET: Mensagens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagem.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // POST: Mensagens/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMensagem,IdRemetente,IdDestinatario,Mensagem1,Data")] Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mensagem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mensagem);
        }

        // GET: Mensagens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagem.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // POST: Mensagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mensagem mensagem = db.Mensagem.Find(id);
            db.Mensagem.Remove(mensagem);
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
