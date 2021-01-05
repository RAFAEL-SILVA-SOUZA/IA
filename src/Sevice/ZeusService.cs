using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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


        public IList<Resposta> RespostasDeUmaPergunta(Pergunta pergunta)
        {
            return zeusDbContext.Respostas.Where(x => x.PerguntaId == pergunta.Id).ToList();
        }

        public (Resposta resposta, bool respondido) Perguntar(Pergunta pergunta)
        {

            var _pergunta = zeusDbContext.Perguntas.FirstOrDefault(x => x.Descricao.Contains(pergunta.Descricao));
            if (_pergunta != null) pergunta = _pergunta;

            var respostas = new List<Resposta>();

            if (pergunta.Id == 0)
                respostas = zeusDbContext.Respostas.Include(x => x.Pergunta).Where(x => x.Pergunta.Descricao.Contains(pergunta.Descricao)).ToList();
            else
                respostas = zeusDbContext.Respostas.AsQueryable().Where(x => x.PerguntaId == pergunta.Id).ToList();

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

            if (!respostas.Any())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Olha, tentei achar algo para te ajudar, mas não consegui! Deseja buscar na web? sim(s) não(n)");
                Console.ForegroundColor = ConsoleColor.White;
                var termo = Console.ReadLine();

                if (termo.ToLower().Equals("s"))
                {
                    respostas = GetWiki(pergunta).Result.ToList();

                    if (!respostas.Any())
                    {
                        return (new Resposta("É, ainda assim não achei nada, quer me ensinar? sim(s) não(n)"), false);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Achei aqui {respostas.Count} alternativas para a sua pergunta, escolha uma! alternativas (1,{respostas.Count}) continuar(s)");
                        Console.ForegroundColor = ConsoleColor.White;
                        termo = Console.ReadLine();

                        var list = new List<int>();
                        for (int i = 0; i < respostas.Count; i++) list.Add(i);

                        while (!termo.ToLower().Equals("s") && list.Any(x => x == int.Parse(termo)))
                        {
                            Console.WriteLine(respostas[int.Parse(termo)-1].Descricao);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Deseja ver mais alternativas? alternativas (1,{respostas.Count}) continuar(s)");
                            Console.ForegroundColor = ConsoleColor.White;
                            termo = Console.ReadLine();
                        }

                        return (new Resposta("Foi bom te ajudar! ;)"), true);
                    }
                }
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
            return zeusDbContext.Respostas.AsQueryable().Where(x => chars.Contains(x.Descricao)).ToList();

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

        //TODO:Melhorar
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

        public async Task<IList<Resposta>> GetWiki(Pergunta pergunta)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://pt.wikipedia.org/w/api.php");
            var response = await client.GetAsync($"?action=query&list=search&srsearch={pergunta.Descricao.Replace(" ", "%20")}&format=json");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var dados = JsonConvert.DeserializeObject<Rootobject>(result);
                var list = new List<Resposta>();

                foreach (var item in dados.query.search)
                {
                    list.Add(new Resposta(StripHTML(item.snippet)));

                    var resposta = new Resposta(StripHTML(item.snippet));
                    Ensinar(resposta, pergunta);
                }

                return await Task.FromResult(list);
            }

            return await Task.FromResult(new List<Resposta>());
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty).Replace("&quot;", "");
        }
    }
}



public class Rootobject
{
    public Query query { get; set; }
}


public class Query
{
    public Search[] search { get; set; }
}


public class Search
{
    public string snippet { get; set; }

}
