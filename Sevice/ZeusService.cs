using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeus.Data;
using Zeus.Domain;

namespace Zeus.Sevice
{
    public class ZeusService
    {
        private readonly ZeusDbContext zeusDbContext;

        public ZeusService()
        {
            zeusDbContext = new ZeusDbContext();
        }

        public (Resposta resposta, bool respondido) Perguntar(Pergunta pergunta)
        {

            var _pergunta = zeusDbContext.Perguntas.FirstOrDefault(x => x.Descricao.Contains(pergunta.Descricao));
            if (_pergunta != null) pergunta = _pergunta;

            var respostas = new List<Resposta>();

            if (pergunta.Id == 0)
                respostas = zeusDbContext.Respostas.Include(x => x.Pergunta).Where(x => x.Pergunta.Descricao.Contains(pergunta.Descricao)).ToList();
            else
                respostas = zeusDbContext.Respostas.Where(x => x.PerguntaId == pergunta.Id).ToList();

            if (!respostas.Any())
            {
                if (pergunta.Id == 0)
                    respostas = zeusDbContext.Respostas.Include(x => x.Pergunta).Where(x => x.Pergunta.Descricao.Contains(RemoverAcentos(pergunta.Descricao))).ToList();
            }

            if (!respostas.Any())
            {
                if (pergunta.Id == 0)
                    respostas = zeusDbContext.Respostas.Include(x => x.Pergunta).Where(x => x.Pergunta.Descricao.Contains(TrocaAbreviacoes(pergunta.Descricao))).ToList();
            }

            if (respostas.Any()) return (ObterRespostaAleatoria(respostas), true);

            return (new Resposta("Ainda não sei responder sua pergunta, quer me ensinar? sim(s) não(n)"), false);
        }

        public void Ensinar(Resposta resposta, Pergunta pergunta)
        {

            var _pergunta = zeusDbContext.Perguntas.FirstOrDefault(x => x.Descricao.Contains(pergunta.Descricao));
            if (_pergunta != null) pergunta = _pergunta;

            pergunta.Respostas.Add(resposta);

            if (pergunta.Id == 0)
                zeusDbContext.Perguntas.Add(pergunta);
            else
                zeusDbContext.Perguntas.Update(pergunta);

            zeusDbContext.SaveChanges();
        }

        public IList<Resposta> ReslizarRespostaMaisProfunda(string pergunta)
        {
            var chars = pergunta.Split(' ');
            return zeusDbContext.Respostas.Where(x => chars.Contains(x.Descricao)).ToList();

        }

        public static string RemoverAcentos(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return texto;
        }

        public static string TrocaAbreviacoes(string texto)
        {
            string comAcentos = "você";
            string semAcentos = "vc";

            texto = texto.Replace(comAcentos, semAcentos);

            return texto;
        }

        public Resposta ObterRespostaAleatoria(IList<Resposta> respostas)
        {
            var rdn = respostas.OrderBy(a => Guid.NewGuid()).ToList();
            return rdn.FirstOrDefault();
        }
    }
}
