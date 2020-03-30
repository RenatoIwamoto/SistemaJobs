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
    public class CandidatosController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Candidatos
        public ActionResult Index()
        {
            var candidato = db.Candidato.Include(c => c.Funcionario).Include(c => c.VagaProjeto);
            return View(candidato.ToList());
        }

        // GET: Candidatos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidato candidato = db.Candidato.Find(id);
            if (candidato == null)
            {
                return HttpNotFound();
            }

            candidato.Funcionario.Telefone = Convert.ToUInt64(candidato.Funcionario.Telefone).ToString(@"\(00\)00000\-0000");

            return View(candidato);
        }

        // GET: Candidatos/Create
        public ActionResult Create()
        {
            //ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome");
            //ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo");

            var idVaga = Convert.ToInt32(Request["vagaId"]);

            var cargos = db.Cargos.Where(s => s.IdVagaProjeto == idVaga && s.Status == 1);

            ViewBag.ListaCargos = cargos.ToList();
            ViewBag.IdFuncionario = Request["funcId"];
            ViewBag.IdVagaProjeto = Request["vagaId"];

            return View();
        }

        // POST: Candidatos/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCandidato,IdVagaProjeto,IdFuncionario,Cargo")] Candidato candidato)
        {
            var cargo = Request.Form["Cargo"];

            if (ModelState.IsValid)
            {
                candidato.VagaDesejada = cargo;
                db.Candidato.Add(candidato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", candidato.IdFuncionario);
            //ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", candidato.IdVagaProjeto);

            //ViewBag.IdFuncionario = Request["funcId"];
            //ViewBag.IdVagaProjeto = Request["vagaId"];

            return View(candidato);
        }

        // GET: Candidatos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidato candidato = db.Candidato.Find(id);
            if (candidato == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", candidato.IdFuncionario);
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", candidato.IdVagaProjeto);
            return View(candidato);
        }

        // POST: Candidatos/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCandidato,IdVagaProjeto,IdFuncionario")] Candidato candidato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", candidato.IdFuncionario);
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", candidato.IdVagaProjeto);
            return View(candidato);
        }

        // GET: Candidatos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidato candidato = db.Candidato.Find(id);
            if (candidato == null)
            {
                return HttpNotFound();
            }
            return View(candidato);
        }

        // POST: Candidatos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Candidato candidato = db.Candidato.Find(id);
            db.Candidato.Remove(candidato);
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
