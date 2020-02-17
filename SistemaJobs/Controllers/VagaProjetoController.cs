using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SistemaJobs;
 
namespace SistemaJobs.Controllers
{
    public class VagaProjetoController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: VagaProjeto
        public ViewResult Index(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var vagas = from s in db.VagaProjeto select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                vagas = vagas.Where(s => s.Titulo.ToUpper().Contains(searchString.ToUpper()));
            }

            vagas = vagas.OrderByDescending(s => s.Titulo);
            var pagamento = vagas.Where(s => s.Cargos.Equals(null)); 

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(vagas.ToPagedList(pageNumber, pageSize));
        }

        // GET: VagaProjeto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(id);
            if (vagaProjeto == null)
            {
                return HttpNotFound();
            }
            return View(vagaProjeto);
        }

        // GET: VagaProjeto/Create
        public ActionResult Create()
        {
            ViewBag.IdCargos = new SelectList(db.Cargos, "IdCargos", "Nome");
            return View();
        }

        // POST: VagaProjeto/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdVagaProjeto,Titulo,Descricao,SalarioOrcamento,Competencias,QtdVagas,Cidade,Estado,RegimeContratacao,DataFinal,Cargos")] VagaProjeto vagaProjeto)
        {
            if (ModelState.IsValid)
            {
                //db.Cargos.Add(vagaProjeto.Cargos);
                db.VagaProjeto.Add(vagaProjeto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCargos = new SelectList(db.Cargos, "IdCargos", "Nome", vagaProjeto.IdCargos);
            return View(vagaProjeto);
        }

        // GET: VagaProjeto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(id);
            if (vagaProjeto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCargos = new SelectList(db.Cargos, "IdCargos", "Nome", vagaProjeto.IdCargos);
            return View(vagaProjeto);
        }

        // POST: VagaProjeto/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdVagaProjeto,Titulo,Descricao,SalarioOrcamento,Competencias,QtdVagas,Cidade,Estado,RegimeContratacao,DataFinal,IdCargos")] VagaProjeto vagaProjeto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vagaProjeto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCargos = new SelectList(db.Cargos, "IdCargos", "Nome", vagaProjeto.IdCargos);
            return View(vagaProjeto);
        }

        // GET: VagaProjeto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(id);
            if (vagaProjeto == null)
            {
                return HttpNotFound();
            }
            return View(vagaProjeto);
        }

        // POST: VagaProjeto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(id);
            db.VagaProjeto.Remove(vagaProjeto);
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
