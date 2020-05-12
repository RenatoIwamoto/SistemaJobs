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
    [Authorize]
    public class MensagensController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: Mensagens
        //public ActionResult Index()
        //{
        //    var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
        //    var idDestinatario = Convert.ToInt32(Request["idDestinatario"]);
        //    var tipoUsuarioLogado = Session["tipoUsuario"].ToString();
        //    var tipoDestinatario = Request["tipo"] == "funcionario" ? "empresa" : "funcionario";

        //    ViewBag.ImagemRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Imagem.ToString();
        //    ViewBag.NomeRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Nome.ToString();
        //    ViewBag.ImagemDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Imagem.ToString();
        //    ViewBag.NomeDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Nome.ToString();

        //    return View(db.Mensagem.ToList());
        //}

        // GET: Mensagens
        public ActionResult Index()
        {
            var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var idDestinatario = Convert.ToInt32(Request["idDestinatario"]);
            var tipoUsuarioLogado = Session["tipoUsuario"].ToString();
            var tipoDestinatario = Request["tipo"] == "funcionario" ? "empresa" : "funcionario";
            //var idConversa = Request["idConversa"] == null ? 0 : Convert.ToInt32(Request["IdConversa"]);

            ViewBag.ImagemRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Imagem.ToString();
            ViewBag.NomeRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Nome.ToString();
            ViewBag.ImagemDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Imagem.ToString();
            ViewBag.NomeDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Nome.ToString();
            ViewBag.TipoDestinatario = tipoDestinatario;

            var mensagens = db.Mensagem.Where(m => m.IdRemetente == idUsuarioLogado && m.IdDestinatario == idDestinatario || m.IdRemetente == idDestinatario && m.IdDestinatario == idUsuarioLogado /*&& m.Conversa.IdConversa == idConversa*/).ToList();
            var msgNaoLidas = mensagens.Where(ms => ms.IdDestinatario == idUsuarioLogado && ms.TipoUsuario != tipoUsuarioLogado && ms.DestinatarioLeu == 0).ToList();

            if (msgNaoLidas.FirstOrDefault() != null)
                SalvarLeitura(msgNaoLidas);

            return View(mensagens);
        }

        public void SalvarLeitura(List<Mensagem> mensagens)
        {
            foreach (var item in mensagens)
            {
                Mensagem msg = db.Mensagem.FirstOrDefault(m => m.IdMensagem == item.IdMensagem);
                msg.DestinatarioLeu = 1;
                db.SaveChanges();
            }
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
            //var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            //var idDestinatario = Convert.ToInt32(Request["idDestinatario"]);
            //var tipoUsuarioLogado = Session["tipoUsuario"].ToString();
            //var tipoDestinatario = Request["tipo"] == "funcionario" ? "empresa" : "funcionario";

            //ViewBag.ImagemRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Imagem.ToString();
            //ViewBag.NomeRemetente = tipoUsuarioLogado == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idUsuarioLogado).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idUsuarioLogado).Nome.ToString();
            //ViewBag.ImagemDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Imagem.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Imagem.ToString();
            //ViewBag.NomeDestinatario = tipoDestinatario == "funcionario" ? db.Funcionario.FirstOrDefault(f => f.IdFuncionario == idDestinatario).Nome.ToString() : db.Empresa.FirstOrDefault(e => e.IdEmpresa == idDestinatario).Nome.ToString();

            return PartialView("Create");
            //return View();
        }

        // POST: Mensagens/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "IdMensagem,IdRemetente,IdDestinatario,Mensagem1,Data,IdConversa")]*/ Mensagem mensagem)
        {
            mensagem.IdDestinatario = Convert.ToInt32(Request["idDestinatario"]);
            mensagem.IdConversa = null;
            mensagem.Data = DateTime.Now;
            mensagem.TipoUsuario = Session["tipoUsuario"].ToString();
            mensagem.DestinatarioLeu = 0;

            var verificaConversa = db.Mensagem.FirstOrDefault(c => c.IdDestinatario == mensagem.IdDestinatario && c.IdRemetente == mensagem.IdRemetente || c.IdDestinatario == mensagem.IdRemetente && c.IdRemetente == mensagem.IdDestinatario);

            if (verificaConversa == null)
                mensagem.IdConversa = SalvarConversa(mensagem);
            else
                mensagem.IdConversa = verificaConversa.IdConversa;
 
            //if (ModelState.IsValid)
            //{
            db.Mensagem.Add(mensagem);
            db.SaveChanges();
            //return RedirectToAction("Index");
            //}
            
            return PartialView("Create");
        }

        public int SalvarConversa(Mensagem mensagem)
        {
            Conversa conversa = new Conversa();

            conversa.IdRemetenteInicial = mensagem.IdRemetente;
            conversa.TipoRemetenteInicial = Session["tipoUsuario"].ToString();
            conversa.Data = DateTime.Now;

            db.Conversa.Add(conversa);
            db.SaveChanges();

            return conversa.IdConversa;
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
