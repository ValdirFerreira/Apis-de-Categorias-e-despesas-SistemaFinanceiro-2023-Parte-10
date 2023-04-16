using Domain.Interfaces.ICategoria;
using Domain.Interfaces.InterfaceServicos;
using Entities.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Servicos
{
    public class CategoriaServico : ICategoriaServico
    {

        private readonly InterfaceCategoria _interfaceCategoria;
        public CategoriaServico(InterfaceCategoria interfaceCategoria)
        {
            _interfaceCategoria = interfaceCategoria;
        }

        public async Task AdicionarCategoria(Categoria catagoria)
        {
            var valido = catagoria.ValidarPropriedadeString(catagoria.Nome, "Nome");
            if (valido)
                await _interfaceCategoria.Add(catagoria);
        }

        public async Task AtualizarCategoria(Categoria catagoria)
        {
            var valido = catagoria.ValidarPropriedadeString(catagoria.Nome, "Nome");
            if (valido)
                await _interfaceCategoria.Update(catagoria);
        }
    }
}
