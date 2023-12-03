using Domain.Interfaces.ISistemaFinanceiro;
using Entities.Entidades;
using Infra.Configuracao;
using Infra.Repositorio.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositorio
{
    public class RepositorioSistemaFinanceiro : RepositoryGenerics<SistemaFinanceiro>, InterfaceSistemaFinanceiro
    {

        private readonly DbContextOptions<ContextBase> _OptionsBuilder;

        public RepositorioSistemaFinanceiro()
        {
            _OptionsBuilder = new DbContextOptions<ContextBase>();
        }

        public async Task<bool> ExecuteCopiaDespesasSistemafinanceiro()
        {
            var listSistemaFinanceiro = new List<SistemaFinanceiro>();


            try
            {

                using (var banco = new ContextBase(_OptionsBuilder))
                {
                    listSistemaFinanceiro = await banco.SistemaFinanceiro.Where(s => s.GerarCopiaDespesa).ToListAsync();
                }


                foreach (var item in listSistemaFinanceiro)
                {

                    using (var banco = new ContextBase(_OptionsBuilder))
                    {

                        var dataatual = DateTime.Now;
                        var mes = dataatual.Month;
                        var ano = dataatual.Year;


                        var despesaJaExiste = await (from d in banco.Despesa
                                                     join c in banco.Categoria on d.IdCategoria equals c.Id
                                                     where c.IdSistema == item.Id
                                                     && d.Mes == mes
                                                     && d.Ano == ano
                                                     select d.Id).AnyAsync();


                        if (!despesaJaExiste)
                        {

                            var despesasSistem = await (from d in banco.Despesa
                                                        join c in banco.Categoria on d.IdCategoria equals c.Id
                                                        where c.IdSistema == item.Id
                                                        && d.Mes == item.MesCopia
                                                        && d.Ano == item.AnoCopia
                                                        select d).ToListAsync();

                            despesasSistem.ForEach(d =>
                            {
                                d.DataVencimento = new DateTime(ano, mes, d.DataVencimento.Day);
                                d.Mes = mes;
                                d.Ano = ano;
                                d.DataAlteracao = DateTime.MinValue;
                                d.DataCadastro = dataatual;
                                d.DataPagamento = DateTime.MinValue;
                                d.Pago = false;
                            });

                            if (despesasSistem.Any())
                            {
                                banco.Despesa.AddRange(despesasSistem);
                                await banco.SaveChangesAsync();
                            }


                        }

                    }

                }

            }
            catch (Exception)
            {
                return false;
            }


            return true;
        }

        public async Task<IList<SistemaFinanceiro>> ListaSistemasUsuario(string emailUsuario)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await
                   (from s in banco.SistemaFinanceiro
                    join us in banco.UsuarioSistemaFinanceiro on s.Id equals us.IdSistema
                    where us.EmailUsuario.Equals(emailUsuario)
                    select s).AsNoTracking().ToListAsync();
            }
        }
    }
}
