using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PagedList;
using SistemaJobs.ViewModel;

namespace SistemaJobs.Controllers
{
    [Authorize]
    public class VagaProjetoController : Controller
    {
        private HiredItEntities db = new HiredItEntities();

        // GET: VagaProjeto
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (Session["tipoUsuario"] == "funcionario")
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

                var vagas = db.VagaProjeto.Where(s => s.QtdVagas >= 1);

                if (!String.IsNullOrEmpty(searchString))
                {
                    vagas = vagas.Where(s => s.Titulo.ToUpper().Contains(searchString.ToUpper()));
                }

                List<VagaProjetoViewModel> listaVagaProjetoVM = new List<VagaProjetoViewModel>();

                var listaVagas = (from vag in vagas
                                  join car in db.Cargos on vag.IdVagaProjeto equals car.IdVagaProjeto
                                  join com in db.Competencias on vag.IdVagaProjeto equals com.IdVagaProjeto
                                  select new
                                  {
                                      vag.IdVagaProjeto,
                                      vag.Titulo,
                                      vag.SalarioOrcamento,
                                      vag.QtdVagas,
                                      vag.TipoVaga
                                  }).OrderByDescending(s => s.IdVagaProjeto).ToList();

                var list = listaVagas.DistinctBy(s => s.IdVagaProjeto);


                foreach (var item in list)
                {
                    VagaProjetoViewModel cliVM = new VagaProjetoViewModel(); //ViewModel
                    cliVM.VagaProjetoViewModelId = item.IdVagaProjeto;
                    cliVM.Titulo = item.Titulo;
                    cliVM.SalarioOrcamento = item.SalarioOrcamento;
                    cliVM.QtdVagas = item.QtdVagas;
                    cliVM.TipoVaga = item.TipoVaga;
                    listaVagaProjetoVM.Add(cliVM);
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);

                return View(listaVagaProjetoVM.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var IdEmpresa = Convert.ToInt32(Session["usuarioLogadoID"]);
                var vagas = db.VagaProjeto.Where(s => s.IdEmpresa == IdEmpresa && s.TipoVaga == "0");

                List<VagaProjetoViewModel> listaVagaProjetoVM = new List<VagaProjetoViewModel>();

                var listaVagas = (from vag in vagas
                                  join car in db.Cargos on vag.IdVagaProjeto equals car.IdVagaProjeto
                                  join com in db.Competencias on vag.IdVagaProjeto equals com.IdVagaProjeto
                                  select new
                                  {
                                      vag.IdVagaProjeto,
                                      vag.Titulo,
                                      vag.SalarioOrcamento,
                                      vag.QtdVagas,
                                      vag.TipoVaga
                                  }).OrderByDescending(s => s.IdVagaProjeto).ToList();

                var list = listaVagas.DistinctBy(s => s.IdVagaProjeto);

                foreach (var item in list)
                {
                    VagaProjetoViewModel cliVM = new VagaProjetoViewModel(); //ViewModel
                    cliVM.VagaProjetoViewModelId = item.IdVagaProjeto;
                    cliVM.Titulo = item.Titulo;
                    cliVM.SalarioOrcamento = item.SalarioOrcamento;
                    cliVM.QtdVagas = item.QtdVagas;
                    cliVM.TipoVaga = item.TipoVaga;
                    listaVagaProjetoVM.Add(cliVM);
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);

                return View(listaVagaProjetoVM.ToPagedList(pageNumber, pageSize));
            }
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

            var idUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);
            var tipoUsuarioLogado = Session["tipoUsuario"];

            if (tipoUsuarioLogado.ToString() == "funcionario" || (tipoUsuarioLogado.ToString() == "empresa" && vagaProjeto.TipoVaga == "0" && vagaProjeto.IdEmpresa == idUsuarioLogado))
            {

                var cargos = db.Cargos.Where(s => s.IdVagaProjeto == id);
                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == id);
                var candidato = db.Candidato.FirstOrDefault(c => c.IdFuncionario == idUsuarioLogado && c.IdVagaProjeto == id);

                var nomeEmpresa = vagaProjeto.Empresa.Nome;
                var imagemEmpresa = vagaProjeto.Empresa.Imagem;

                ViewBag.ListaCargos = cargos.ToList();
                ViewBag.ListaCompetencias = competencias.ToList();
                ViewBag.Candidato = candidato;
                ViewBag.IdVaga = id;
                ViewBag.NomeEmpresa = nomeEmpresa;
                ViewBag.Imagem = imagemEmpresa;

                VagaProjetoViewModel cliVM = new VagaProjetoViewModel(); //ViewModel
                cliVM.Titulo = vagaProjeto.Titulo;
                cliVM.Descricao = vagaProjeto.Descricao;
                cliVM.SalarioOrcamento = vagaProjeto.SalarioOrcamento;
                cliVM.DataFinal = vagaProjeto.DataFinal;
                cliVM.TipoVaga = vagaProjeto.TipoVaga;

                return View(cliVM);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: VagaProjeto/Create
        public ActionResult Create(string tipoVagaParam)
        {
            if (Session["tipoUsuario"].ToString() == "empresa")
            {
                ViewBag.tipoVagaParam = tipoVagaParam;
                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult SalvarFuncionarioProjeto()
        {
            var idFuncionario = Convert.ToInt32(Request["idFuncionario"]);
            var idVaga = Convert.ToInt32(Request["idVaga"]);

            FuncionarioProjeto funcionarioProjeto = new FuncionarioProjeto();

            funcionarioProjeto.IdFuncionario = idFuncionario;
            funcionarioProjeto.IdVagaProjeto = idVaga;
            funcionarioProjeto.Ativo = 1;
            db.FuncionarioProjeto.Add(funcionarioProjeto);
            db.SaveChanges();

            return RedirectToAction("Details", "Candidatos", new { id = idFuncionario });
        }

        // POST: VagaProjeto/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Titulo,Descricao,SalarioOrcamento,QtdVagas,Cidade,Estado,RegimeContratacao,DataFinal,TipoVaga,ListaCargos,ListaCompetencias")] VagaProjetoViewModel viewModel)
        {

            if (viewModel.QtdVagas == 0 && viewModel.ListaCargos == null && viewModel.ListaCompetencias == null)
            {
                ViewBag.Quantidade = "Insira a quantidade de vagas disponíveis";
                ViewBag.message1 = "Competencias é um campo obrigatório";
                ViewBag.message2 = "Cargos é um campo obrigatório";

                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();

                return View(viewModel);
            }

            else if (viewModel.QtdVagas == 0)
            {
                ViewBag.Quantidade = "Insira a quantidade de vagas disponíveis";
                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                return View(viewModel);
            }

            else if (viewModel.ListaCargos == null || viewModel.ListaCompetencias == null)
            {
                ViewBag.message1 = "Competencias é um campo obrigatório";
                ViewBag.message2 = "Cargos é um campo obrigatório";
                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var IdEmpresa = Convert.ToInt32(Session["usuarioLogadoID"]);

                Empresa empresa = db.Empresa.Find(IdEmpresa);

                VagaProjeto vagas = new VagaProjeto();
                vagas.Titulo = viewModel.Titulo;
                vagas.Descricao = viewModel.Descricao;
                vagas.SalarioOrcamento = viewModel.SalarioOrcamento;
                vagas.QtdVagas = viewModel.QtdVagas;
                vagas.Cidade = viewModel.Cidade;
                vagas.Estado = viewModel.Estado;
                vagas.RegimeContratacao = viewModel.RegimeContratacao;
                vagas.DataFinal = viewModel.DataFinal;
                vagas.TipoVaga = viewModel.TipoVaga;
                vagas.Empresa = empresa;

                foreach (var item in viewModel.ListaCargos)
                {
                    Cargos cargos = new Cargos();
                    cargos.Nome = item;
                    cargos.Status = 1;
                    cargos.VagaProjeto1 = vagas;
                    db.Cargos.Add(cargos);
                }

                foreach (var item in viewModel.ListaCompetencias)
                {
                    Competencias competencias = new Competencias();
                    competencias.NomeCompetencia = item;
                    competencias.VagaProjeto = vagas;
                    db.Competencias.Add(competencias);
                }

                db.SaveChanges();

                if (vagas.TipoVaga == "0")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index2", "MeusProjetos", new { });
                }
            }

            PopularDdlEstado();
            PopularDdlRegimeContratacao();
            PopularDdlTipoVaga();
            return View(viewModel);
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

            var IdUsuarioLogado = Convert.ToInt32(Session["usuarioLogadoID"]);

            if (Session["tipoUsuario"] == "empresa" && vagaProjeto.IdEmpresa == IdUsuarioLogado)
          {
                VagaProjetoViewModel cliVM = new VagaProjetoViewModel(); //ViewModel
                cliVM.VagaProjetoViewModelId = vagaProjeto.IdVagaProjeto;
                cliVM.Titulo = vagaProjeto.Titulo;
                cliVM.Descricao = vagaProjeto.Descricao;
                cliVM.SalarioOrcamento = (int)vagaProjeto.SalarioOrcamento;
                cliVM.QtdVagas = vagaProjeto.QtdVagas;
                cliVM.Cidade = vagaProjeto.Cidade;
                cliVM.Estado = vagaProjeto.Estado;
                cliVM.RegimeContratacao = vagaProjeto.DataFinal.ToString("dd/MM/yyyy");
                cliVM.DataFinal = vagaProjeto.DataFinal;
                cliVM.TipoVaga = vagaProjeto.TipoVaga;


                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == id);
                ViewBag.ListaCompetencias = competencias.ToList();

                var cargos = db.Cargos.Where(s => s.IdVagaProjeto == id);
                ViewBag.ListaCargos = cargos.ToList();

                ViewBag.EstadoList = vagaProjeto.Estado;
                ViewBag.RegimeContratacaoList = vagaProjeto.RegimeContratacao;

                if (vagaProjeto.TipoVaga == "0")
                {
                    ViewBag.TipoVagaList = "Vaga de Emprego";
                }
                else
                {
                    ViewBag.TipoVagaList = "Projeto Freelancer";
                }

                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                PopularCargoCompetencia(vagaProjeto.IdVagaProjeto);

                return View(cliVM);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: VagaProjeto/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VagaProjetoViewModelId,Titulo,Descricao,SalarioOrcamento,QtdVagas,Cidade,Estado,RegimeContratacao,DataFinal,TipoVaga,ListaCargos,ListaCompetencias")] VagaProjetoViewModel viewModel)
        {
            string tipoVagaAux = null;

            if (viewModel.ListaCompetencias == null && viewModel.ListaCargos == null)
            {
                ViewBag.message1 = "Competencias é um campo obrigatório";
                ViewBag.message2 = "Cargos é um campo obrigatório";
                PopularCargoCompetencia(viewModel.VagaProjetoViewModelId);
                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();

                ViewBag.EstadoList = viewModel.Estado;
                ViewBag.RegimeContratacaoList = viewModel.RegimeContratacao;
                ViewBag.TipoVagaList = viewModel.TipoVaga;

                ViewBag.ListaCargos = new List<SelectListItem>();
                ViewBag.ListaCompetencias = new List<SelectListItem>();

                return View(viewModel);
            }
            else if (viewModel.ListaCompetencias == null)
            {
                ViewBag.message1 = "Competencias é um campo obrigatório";
                PopularCargoCompetencia(viewModel.VagaProjetoViewModelId);

                var cargos1 = db.Cargos.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);

                ViewBag.EstadoList = viewModel.Estado;
                ViewBag.RegimeContratacaoList = viewModel.RegimeContratacao;
                ViewBag.TipoVagaList = viewModel.TipoVaga;

                ViewBag.ListaCargos = cargos1.ToList();
                ViewBag.ListaCompetencias = new List<SelectListItem>();

                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                return View(viewModel);
            }
            else if (viewModel.ListaCargos == null)
            {
                ViewBag.message2 = "Cargos é um campo obrigatório";
                PopularCargoCompetencia(viewModel.VagaProjetoViewModelId);

                var competencia1 = db.Competencias.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);

                ViewBag.EstadoList = viewModel.Estado;
                ViewBag.RegimeContratacaoList = viewModel.RegimeContratacao;
                ViewBag.TipoVagaList = viewModel.TipoVaga;

                ViewBag.ListaCargos = new List<SelectListItem>();
                ViewBag.ListaCompetencias = competencia1.ToList();

                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();
                return View(viewModel);
            }

            if (viewModel.TipoVaga == "Vaga de Emprego")
            {
                tipoVagaAux = "0";
            }
            else if (viewModel.TipoVaga == "Projeto Freelancer")
            {
                tipoVagaAux = "1";
            }
            else
            {
                ViewBag.message3 = "Apenas Vaga de Emprego e Projeto Freelancer serão aceitos como tipos válidos";

                PopularCargoCompetencia(viewModel.VagaProjetoViewModelId);
                PopularDdlEstado();
                PopularDdlRegimeContratacao();
                PopularDdlTipoVaga();

                ViewBag.EstadoList = viewModel.Estado;
                ViewBag.RegimeContratacaoList = viewModel.RegimeContratacao;
                ViewBag.TipoVagaList = viewModel.TipoVaga;

                var cargos1 = db.Cargos.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);
                ViewBag.ListaCargos = cargos1.ToList();

                var competencia1 = db.Competencias.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);
                ViewBag.ListaCompetencias = competencia1.ToList();

                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var IdEmpresa = Convert.ToInt32(Session["usuarioLogadoID"]);

                VagaProjeto vagas = new VagaProjeto();
                vagas.IdVagaProjeto = viewModel.VagaProjetoViewModelId;
                vagas.Titulo = viewModel.Titulo;
                vagas.Descricao = viewModel.Descricao;
                vagas.SalarioOrcamento = viewModel.SalarioOrcamento;
                vagas.QtdVagas = viewModel.QtdVagas;
                vagas.Cidade = viewModel.Cidade;
                vagas.Estado = viewModel.Estado;
                vagas.RegimeContratacao = viewModel.RegimeContratacao;
                vagas.DataFinal = viewModel.DataFinal;
                vagas.TipoVaga = tipoVagaAux;
                vagas.IdEmpresa = IdEmpresa;
                db.Entry(vagas).State = EntityState.Modified;


                var cargos = db.Cargos.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);
                var competencias = db.Competencias.Where(s => s.IdVagaProjeto == viewModel.VagaProjetoViewModelId);

                foreach (var item in cargos)
                {
                    var cargo = db.Cargos.FirstOrDefault(cp => cp.IdCargos == item.IdCargos);
                    db.Cargos.Remove(cargo);
                }

                foreach (var item in viewModel.ListaCargos)
                {
                    Cargos cargo2 = new Cargos();
                    cargo2.Nome = item;
                    cargo2.Status = 1;
                    cargo2.VagaProjeto1 = vagas;
                    db.Cargos.Add(cargo2);
                }

                foreach (var item in competencias)
                {
                    var competencia = db.Competencias.FirstOrDefault(cp => cp.IdCompetencias == item.IdCompetencias);
                    db.Competencias.Remove(competencia);
                }

                foreach (var item in viewModel.ListaCompetencias)
                {
                    Competencias competencia2 = new Competencias();
                    competencia2.NomeCompetencia = item;
                    competencia2.VagaProjeto = vagas;
                    db.Competencias.Add(competencia2);
                }

                db.SaveChanges();

                if (vagas.TipoVaga == "0")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index2", "MeusProjetos", new {});
                }
            }

            ViewBag.EstadoList = viewModel.Estado;
            ViewBag.RegimeContratacaoList = viewModel.RegimeContratacao;
            ViewBag.TipoVagaList = viewModel.TipoVaga;

            PopularCargoCompetencia(viewModel.VagaProjetoViewModelId);
            PopularViewModelVagas(viewModel.VagaProjetoViewModelId);
            return View(viewModel);
        }

        protected void PopularViewModelVagas(int id)
        {
            var competencias1 = db.Competencias.Where(s => s.IdVagaProjeto == id);
            ViewBag.ListaCompetencias = competencias1.ToList();

            var cargos1 = db.Cargos.Where(s => s.IdVagaProjeto == id);
            ViewBag.ListaCargos = cargos1.ToList();

            PopularDdlEstado();
            PopularDdlRegimeContratacao();
            PopularDdlTipoVaga();
        }

        //Popula a lista de Cargos e Competências no Select 2
        protected void PopularCargoCompetencia(int id)
        {
            var cargolist = db.Cargos.Select(c => new {
                CargoID = c.IdCargos,
                CargoName = c.Nome,
                CargoVaga = c.IdVagaProjeto
            }).Where(m => m.CargoVaga == id).ToList();

            var competencialist = db.Competencias.Select(c => new {
                CompetenciaID = c.IdCompetencias,
                CompetenciaName = c.NomeCompetencia,
                CompetenciaVaga = c.IdVagaProjeto
            }).Where(m => m.CompetenciaVaga == id).ToList();

            string aux = "";
            foreach (var item in cargolist)
            {
                aux = aux + "," + item.CargoName;
            }

            string[] aux2 = aux.Split(',').ToArray();
            ViewBag.Carg = aux2;

            foreach (var item in competencialist)
            {
                aux = aux + "," + item.CompetenciaName;
            }
            aux2 = aux.Split(',').ToArray();
            ViewBag.Comp = aux2;
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

        //Popula a lista de Estados brasileiros
        protected void PopularDdlRegimeContratacao()
        {
            List<SelectListItem> ddlRegimeContratacaoItems = new List<SelectListItem>();
            ddlRegimeContratacaoItems.Add(new SelectListItem { Value = "CLT - Carteira assinada", Text = "CLT - Carteira assinada" });
            ddlRegimeContratacaoItems.Add(new SelectListItem { Value = "PJ - Pessoa Jurídica", Text = "PJ - Pessoa Jurídica" });
            ddlRegimeContratacaoItems.Add(new SelectListItem { Value = "Freelancer", Text = "Freelancer" });
            ViewBag.RegimeContratacao = ddlRegimeContratacaoItems;
        }

        //Popula a lista tipo de vaga
        protected void PopularDdlTipoVaga()
        {
            List<SelectListItem> ddlTipoVagaItems = new List<SelectListItem>();
            ddlTipoVagaItems.Add(new SelectListItem { Value = "0", Text = "Vaga de Emprego" });
            ddlTipoVagaItems.Add(new SelectListItem { Value = "1", Text = "Projeto Freelancer" });
            ViewBag.TipoVaga = ddlTipoVagaItems;
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








