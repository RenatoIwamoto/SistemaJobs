using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PagedList;
using SistemaJobs;

namespace SistemaJobs.Controllers
{
    [Authorize]
    public class MeusProjetosController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: MeusProjetos
        public ActionResult Index(int? page)
        {
            var funcionarioProjeto = db.FuncionarioProjeto.Include(f => f.Funcionario).Include(f => f.VagaProjeto);
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var TipoUsuarioLogado = Session["tipoUsuario"];

            List<FuncionarioProjeto> listaVagaProjetoVM = new List<FuncionarioProjeto>();

            var listaVagas = (from vag in funcionarioProjeto
                              select new
                              {
                                  vag.IdFuncionarioProjeto,
                                  vag.IdFuncionario,
                                  vag.Funcionario,
                                  vag.VagaProjeto
                              }).OrderByDescending(s => s.IdFuncionarioProjeto).ToList();

            var list = listaVagas.DistinctBy(s => s.IdFuncionarioProjeto);

            if (TipoUsuarioLogado == "empresa")
            {
                list = list.Where(s => s.VagaProjeto.IdEmpresa == IdUsuarioLogado);
            }
            else
            {
                list = list.Where(s => s.IdFuncionario == IdUsuarioLogado);
            }

            foreach (var item in list)
            {
                FuncionarioProjeto cliVM = new FuncionarioProjeto(); //ViewModel
                cliVM.IdFuncionarioProjeto = item.IdFuncionarioProjeto;
                cliVM.Funcionario = item.Funcionario;
                cliVM.VagaProjeto = item.VagaProjeto;
                listaVagaProjetoVM.Add(cliVM);
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(listaVagaProjetoVM.ToPagedList(pageNumber, pageSize));
        }

        // GET: MeusProjetos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioProjeto funcionarioProjeto = db.FuncionarioProjeto.Find(id);
            if (funcionarioProjeto == null)
            {
                return HttpNotFound();
            }

            var cargos = db.Cargos.Where(s => s.IdVagaProjeto == funcionarioProjeto.IdVagaProjeto);
            var competencias = db.Competencias.Where(s => s.IdVagaProjeto == funcionarioProjeto.IdVagaProjeto);

            //var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            ViewBag.candidatos = db.FuncionarioProjeto.Where(s => s.VagaProjeto.IdVagaProjeto == funcionarioProjeto.IdVagaProjeto);

            ViewBag.ListaCargos = cargos.ToList();
            ViewBag.ListaCompetencias = competencias.ToList();

            FuncionarioProjeto cliVM = new FuncionarioProjeto(); //ViewModel
            cliVM.Funcionario = funcionarioProjeto.Funcionario;
            cliVM.VagaProjeto = funcionarioProjeto.VagaProjeto;
            return View(funcionarioProjeto);
        }

        // GET: MeusProjetos/Create
        public ActionResult Create()
        {
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome");
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo");
            return View();
        }

        // POST: MeusProjetos/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFuncionarioProjeto,IdFuncionario,IdVagaProjeto,Ativo")] FuncionarioProjeto funcionarioProjeto)
        {
            if (ModelState.IsValid)
            {
                db.FuncionarioProjeto.Add(funcionarioProjeto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", funcionarioProjeto.IdFuncionario);
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", funcionarioProjeto.IdVagaProjeto);
            return View(funcionarioProjeto);
        }

        // GET: MeusProjetos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioProjeto funcionarioProjeto = db.FuncionarioProjeto.Find(id);
            if (funcionarioProjeto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", funcionarioProjeto.IdFuncionario);
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", funcionarioProjeto.IdVagaProjeto);
            return View(funcionarioProjeto);
        }

        // POST: MeusProjetos/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFuncionarioProjeto,IdFuncionario,IdVagaProjeto,Ativo")] FuncionarioProjeto funcionarioProjeto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(funcionarioProjeto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFuncionario = new SelectList(db.Funcionario, "IdFuncionario", "Nome", funcionarioProjeto.IdFuncionario);
            ViewBag.IdVagaProjeto = new SelectList(db.VagaProjeto, "IdVagaProjeto", "Titulo", funcionarioProjeto.IdVagaProjeto);
            return View(funcionarioProjeto);
        }

        // GET: MeusProjetos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioProjeto funcionarioProjeto = db.FuncionarioProjeto.Find(id);
            if (funcionarioProjeto == null)
            {
                return HttpNotFound();
            }
            return View(funcionarioProjeto);
        }

        // POST: MeusProjetos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FuncionarioProjeto funcionarioProjeto = db.FuncionarioProjeto.Find(id);
            db.FuncionarioProjeto.Remove(funcionarioProjeto);
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
