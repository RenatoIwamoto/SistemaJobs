using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SistemaJobs;
using SistemaJobs.ViewModels;

namespace SistemaJobs.Controllers
{
    public class ConversasController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Conversas
        public ActionResult Index()
        {
            var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            //var idDestinatario = Convert.ToInt32(Request["IdDestinatario"]);
            var tipoUsuarioLogado = Session["tipoUsuario"].ToString();
            var tipoDestinatario = tipoUsuarioLogado == "funcionario" ? "empresa" : "funcionario";

            List<VmUsuarioConversa> cnv = new List<VmUsuarioConversa>();

            //var conversa = from m in db.Mensagem
            //               join c in db.Conversa on m.IdConversa equals c.IdConversa
            //               join f in db.Funcionario on m. equals c.IdConversa
            //               where (m.IdRemetente == idUsuario) || (m.IdDestinatario == idUsuario)
            //               select new { idConversa = c.IdConversa };

            //var conversa = db.Mensagem.Where(m => m.IdRemetente == idUsuarioLogado && m.IdDestinatario == idDestinatario || m.IdRemetente == idDestinatario && m.IdDestinatario == idUsuarioLogado).DistinctBy(c => c.IdConversa);
            var conversa = db.Mensagem.Where(m => m.IdRemetente == idUsuarioLogado && m.TipoUsuario == tipoUsuarioLogado || m.IdDestinatario == idUsuarioLogado && m.TipoUsuario == tipoDestinatario).DistinctBy(c => c.IdConversa);
            //var conversaNaoLida = db.Mensagem.Where(msg => msg.IdDestinatario == idUsuarioLogado && msg.TipoUsuario != tipoUsuarioLogado && msg.DestinatarioLeu == 0).Count();

            foreach (var item in conversa)
            {
                VmUsuarioConversa vmUsu = new VmUsuarioConversa();
                vmUsu.IdConversa = item.IdConversa;
                vmUsu.IdDestinatario = item.IdDestinatario;
                vmUsu.IdRemetenteInicial = item.Conversa.IdRemetenteInicial;
                vmUsu.Usuario = tipoUsuarioLogado == "funcionario" ? db.Empresa.FirstOrDefault(e => e.IdEmpresa == item.IdDestinatario).Nome.ToString() : db.Funcionario.FirstOrDefault(f => f.IdFuncionario == item.IdDestinatario).Nome.ToString();
                vmUsu.NaoLida = db.Mensagem.Where(msg => msg.IdConversa == item.IdConversa && msg.IdDestinatario == idUsuarioLogado && msg.TipoUsuario != tipoUsuarioLogado && msg.DestinatarioLeu == 0).Count();
                cnv.Add(vmUsu);
            }

            return View(cnv);
        }

        // GET: Conversas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversa conversa = db.Conversa.Find(id);
            if (conversa == null)
            {
                return HttpNotFound();
            }
            return View(conversa);
        }

        // GET: Conversas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Conversas/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdConversa,IdRemetenteInicial,TipoRemetenteInicial,Data")] Conversa conversa)
        {
            if (ModelState.IsValid)
            {
                db.Conversa.Add(conversa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(conversa);
        }

        // GET: Conversas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversa conversa = db.Conversa.Find(id);
            if (conversa == null)
            {
                return HttpNotFound();
            }
            return View(conversa);
        }

        // POST: Conversas/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdConversa,IdRemetenteInicial,TipoRemetenteInicial,Data")] Conversa conversa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conversa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(conversa);
        }

        // GET: Conversas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversa conversa = db.Conversa.Find(id);
            if (conversa == null)
            {
                return HttpNotFound();
            }
            return View(conversa);
        }

        // POST: Conversas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Conversa conversa = db.Conversa.Find(id);
            db.Conversa.Remove(conversa);
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
