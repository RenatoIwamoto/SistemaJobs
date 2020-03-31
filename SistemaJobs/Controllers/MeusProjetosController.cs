﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PagedList;
using SistemaJobs;
using SistemaJobs.ViewModel;

namespace SistemaJobs.Controllers
{
    [Authorize]
    public class MeusProjetosController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: MeusProjetos
        public ActionResult Index(int? page)
        {
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var TipoUsuarioLogado = Session["tipoUsuario"];

            if (TipoUsuarioLogado == "funcionario")
            {
            var funcionarioProjeto = db.FuncionarioProjeto.Include(f => f.Funcionario).Include(f => f.VagaProjeto);
            List<FuncionarioProjeto> listaVagaProjetoVM = new List<FuncionarioProjeto>();

                var listaVagas = (from vag in funcionarioProjeto
                                  select new
                                  {
                                      vag.IdFuncionarioProjeto,
                                      vag.IdFuncionario,
                                      vag.Funcionario,
                                      vag.VagaProjeto,
                                      vag.Ativo
                                  }).OrderByDescending(s => s.IdFuncionarioProjeto).ToList();
                var list = listaVagas.DistinctBy(s => s.IdFuncionarioProjeto);
                list = list.Where(s => s.IdFuncionario == IdUsuarioLogado && s.Ativo == 1);

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
            else
            {
                return RedirectToAction("Index2");
            }
        }

        // GET: MeusProjetos
        public ActionResult Index2(int? page)
        {
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var TipoUsuarioLogado = Session["tipoUsuario"];

            if (TipoUsuarioLogado == "empresa")
            {
                var empresaProjeto = db.VagaProjeto.Where(s => s.IdEmpresa == IdUsuarioLogado && s.TipoVaga == "1");
                List<VagaProjetoViewModel> listaVagaProjetoVM = new List<VagaProjetoViewModel>();

                var listaVagas = (from vag in empresaProjeto
                                  select new
                                  {
                                      vag.IdVagaProjeto,
                                      vag.Titulo,
                                      vag.SalarioOrcamento,
                                      vag.QtdVagas,
                                      vag.TipoVaga,
                                      vag.Descricao
                                  }).OrderByDescending(s => s.IdVagaProjeto).ToList();
                var list = listaVagas.DistinctBy(s => s.IdVagaProjeto);

                foreach (var item in list)
                {
                    VagaProjetoViewModel cliVM2 = new VagaProjetoViewModel(); //ViewModel
                    cliVM2.VagaProjetoViewModelId = item.IdVagaProjeto;
                    cliVM2.Titulo = item.Titulo;
                    cliVM2.SalarioOrcamento = item.SalarioOrcamento;
                    cliVM2.QtdVagas = item.QtdVagas;
                    cliVM2.TipoVaga = item.TipoVaga;
                    cliVM2.Descricao = item.Descricao;
                    listaVagaProjetoVM.Add(cliVM2);
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);

                return View(listaVagaProjetoVM.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: MeusProjetos/Details/5
        public ActionResult Details(int? id)
        {
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var TipoUsuarioLogado = Session["tipoUsuario"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioProjeto funcionarioProjeto = db.FuncionarioProjeto.Find(id);
            if (funcionarioProjeto == null)
            {
                return HttpNotFound();
            }

            if (TipoUsuarioLogado == "funcionario" && funcionarioProjeto.IdFuncionario == IdUsuarioLogado && funcionarioProjeto.Ativo == 1)
            {
                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == funcionarioProjeto.IdVagaProjeto);
                ViewBag.ListaCompetencias = competencias.ToList();

                var funcionarioList = db.FuncionarioProjeto.Where(s => s.VagaProjeto.IdVagaProjeto == funcionarioProjeto.IdVagaProjeto && s.Ativo == 1);

                var funcionario_candidato = (from func in funcionarioList
                                             join cad in db.Candidato on func.IdVagaProjeto equals cad.IdVagaProjeto
                                             select cad).ToList();

                ViewBag.candidatos = funcionario_candidato.DistinctBy(s => s.IdCandidato);

                return View(funcionarioProjeto);
            }
            else
            {
                return RedirectToAction("Index2");
            }
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
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var TipoUsuarioLogado = Session["tipoUsuario"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(id);
            if (vagaProjeto == null)
            {
                return HttpNotFound();
            }

            if (TipoUsuarioLogado == "empresa" && IdUsuarioLogado == vagaProjeto.IdEmpresa)
            {
                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == id);
                ViewBag.ListaCompetencias = competencias.ToList();

                string select = "select * from Candidato inner join FuncionarioProjeto on Candidato.IdVagaProjeto = FuncionarioProjeto.IdVagaProjeto and Candidato.IdFuncionario = FuncionarioProjeto.IdFuncionario WHERE FuncionarioProjeto.Ativo = 1 and FuncionarioProjeto.IdVagaProjeto = " + id;
                IEnumerable<Candidato> data = db.Database.SqlQuery<Candidato>(select);
                List<VagaProjetoViewModel> listaVagaProjetoVM = new List<VagaProjetoViewModel>();

                foreach (var item in data)
                {
                    Funcionario funcionario = db.Funcionario.Find(item.IdFuncionario);
                    Candidato candidato = db.Candidato.Find(item.IdCandidato);

                    VagaProjetoViewModel vagaProjetoViewModel2 = new VagaProjetoViewModel();
                    vagaProjetoViewModel2.Candidato = candidato;
                    vagaProjetoViewModel2.Funcionario = funcionario;
                    listaVagaProjetoVM.Add(vagaProjetoViewModel2);
                }
                ViewBag.candidatos = listaVagaProjetoVM;

                VagaProjetoViewModel vagaProjetoViewModel = new VagaProjetoViewModel();
                vagaProjetoViewModel.VagaProjetoViewModelId = vagaProjeto.IdVagaProjeto;
                vagaProjetoViewModel.Titulo = vagaProjeto.Titulo;
                vagaProjetoViewModel.Descricao = vagaProjeto.Descricao;
                vagaProjetoViewModel.SalarioOrcamento = vagaProjeto.SalarioOrcamento;
                vagaProjetoViewModel.QtdVagas = vagaProjeto.QtdVagas;
                vagaProjetoViewModel.Cidade = vagaProjeto.Cidade;
                vagaProjetoViewModel.Estado = vagaProjeto.Estado;
                vagaProjetoViewModel.RegimeContratacao = vagaProjeto.RegimeContratacao;
                vagaProjetoViewModel.DataFinal = vagaProjeto.DataFinal;
                vagaProjetoViewModel.TipoVaga = vagaProjeto.TipoVaga;

                return View(vagaProjetoViewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: MeusProjetos/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VagaProjetoViewModelId,Check")] VagaProjetoViewModel viewModel, IList<int> checks)
        {
            var TipoUsuarioLogado = Session["tipoUsuario"];
            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            VagaProjeto vagaProjeto = db.VagaProjeto.Find(viewModel.VagaProjetoViewModelId);

            if (TipoUsuarioLogado == "empresa" && IdUsuarioLogado == vagaProjeto.IdEmpresa)
            {
                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);
                ViewBag.ListaCompetencias = competencias.ToList();

                VagaProjetoViewModel vagaProjetoViewModel = new VagaProjetoViewModel();
                vagaProjetoViewModel.VagaProjetoViewModelId = vagaProjeto.IdVagaProjeto;
                vagaProjetoViewModel.Titulo = vagaProjeto.Titulo;
                vagaProjetoViewModel.Descricao = vagaProjeto.Descricao;
                vagaProjetoViewModel.SalarioOrcamento = vagaProjeto.SalarioOrcamento;
                vagaProjetoViewModel.QtdVagas = vagaProjeto.QtdVagas;
                vagaProjetoViewModel.Cidade = vagaProjeto.Cidade;
                vagaProjetoViewModel.Estado = vagaProjeto.Estado;
                vagaProjetoViewModel.RegimeContratacao = vagaProjeto.RegimeContratacao;
                vagaProjetoViewModel.DataFinal = vagaProjeto.DataFinal;
                vagaProjetoViewModel.TipoVaga = vagaProjeto.TipoVaga;

                string select = "select * from Candidato inner join FuncionarioProjeto on Candidato.IdVagaProjeto = FuncionarioProjeto.IdVagaProjeto and Candidato.IdFuncionario = FuncionarioProjeto.IdFuncionario WHERE FuncionarioProjeto.Ativo = 1 and FuncionarioProjeto.IdVagaProjeto = " + viewModel.VagaProjetoViewModelId;
                IEnumerable<Candidato> data = db.Database.SqlQuery<Candidato>(select);
                List<VagaProjetoViewModel> listaVagaProjetoVM = new List<VagaProjetoViewModel>();

                foreach (var item in data)
                {
                    Funcionario funcionario = db.Funcionario.Find(item.IdFuncionario);
                    Candidato candidato = db.Candidato.Find(item.IdCandidato);

                    VagaProjetoViewModel vagaProjetoViewModel2 = new VagaProjetoViewModel();
                    vagaProjetoViewModel2.Candidato = candidato;
                    vagaProjetoViewModel2.Funcionario = funcionario;
                    listaVagaProjetoVM.Add(vagaProjetoViewModel2);
                }
                ViewBag.candidatos = listaVagaProjetoVM;

                if (checks != null)
                {
                    foreach (var item in checks)
                    {
                        var funcionarioProjeto = db.FuncionarioProjeto.FirstOrDefault(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId && s.IdFuncionario == item);
                        db.FuncionarioProjeto.Remove(funcionarioProjeto);
                    }

                    foreach (var item in checks)
                    {
                        var IdFuncionarioProjetoAux = db.FuncionarioProjeto.FirstOrDefault(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId && s.IdFuncionario == item);

                        FuncionarioProjeto funcionarioProjeto = new FuncionarioProjeto();
                        funcionarioProjeto.IdFuncionarioProjeto = IdFuncionarioProjetoAux.IdFuncionarioProjeto;
                        funcionarioProjeto.IdFuncionario = item;
                        funcionarioProjeto.IdVagaProjeto = viewModel.VagaProjetoViewModelId;
                        funcionarioProjeto.Ativo = 0;
                        db.FuncionarioProjeto.Add(funcionarioProjeto);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {
                    return View(vagaProjetoViewModel);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
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
