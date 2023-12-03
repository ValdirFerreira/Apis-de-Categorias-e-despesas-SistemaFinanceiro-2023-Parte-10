using Domain.Interfaces.InterfaceServicos;
using Domain.Interfaces.ISistemaFinanceiro;
using Entities.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SistemaFinanceirosController : ControllerBase
    {
        private readonly InterfaceSistemaFinanceiro _InterfaceSistemaFinanceiro;
        private readonly ISistemaFinanceiroServico _ISistemaFinanceiroServico;
        public SistemaFinanceirosController(InterfaceSistemaFinanceiro InterfaceSistemaFinanceiro,
            ISistemaFinanceiroServico ISistemaFinanceiroServico)
        {
            _InterfaceSistemaFinanceiro = InterfaceSistemaFinanceiro;
            _ISistemaFinanceiroServico = ISistemaFinanceiroServico;
        }

        [HttpGet("/api/ListaSistemasUsuario")]
        [Produces("application/json")]
        public async Task<object> ListaSistemasUsuario(string emailUsuario)
        {
            return await _InterfaceSistemaFinanceiro.ListaSistemasUsuario(emailUsuario);
        }

        [HttpPost("/api/AdicionarSistemaFinanceiro")]
        [Produces("application/json")]
        public async Task<object> AdicionarSistemaFinanceiro(SistemaFinanceiro sistemaFinanceiro)
        {
            await _ISistemaFinanceiroServico.AdicionarSistemaFinanceiro(sistemaFinanceiro);

            return sistemaFinanceiro;
        }

        [HttpPut("/api/AtualizarSistemaFinanceiro")]
        [Produces("application/json")]
        public async Task<object> AtualizarSistemaFinanceiro(SistemaFinanceiro sistemaFinanceiro)
        {
            await _ISistemaFinanceiroServico.AtualizarSistemaFinanceiro(sistemaFinanceiro);

            return Task.FromResult(sistemaFinanceiro);
        }


        [HttpGet("/api/ObterSistemaFinanceiro")]
        [Produces("application/json")]
        public async Task<object> ObterSistemaFinanceiro(int id)
        {
            return await _InterfaceSistemaFinanceiro.GetEntityById(id);
        }


        [HttpDelete("/api/DeleteSistemaFinanceiro")]
        [Produces("application/json")]
        public async Task<object> DeleteSistemaFinanceiro(int id)
        {
            try
            {
                var sistemaFinanceiro = await _InterfaceSistemaFinanceiro.GetEntityById(id);

                await _InterfaceSistemaFinanceiro.Delete(sistemaFinanceiro);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        [HttpPost("/api/ExecuteCopiaDespesasSistemafinanceiro")]
        [Produces("application/json")]
        public async Task<object> ExecuteCopiaDespesasSistemafinanceiro()
        {
            return await _InterfaceSistemaFinanceiro.ExecuteCopiaDespesasSistemafinanceiro();
        }


    }
}
