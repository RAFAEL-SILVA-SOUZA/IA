using System;
using System.Collections.Generic;
using System.Linq;
using Zeus.Domain;
using Zeus.Helper;
using Zeus.Sevice;

namespace Zeus
{
    public class Program
    {
        static void Main(string[] args)
        {
            var zeusService = new ZeusService();

            while (true)
            {
                "Como posso ajudá-lo agora?".Escreve();
                var pergunta = Console.ReadLine();

                var resposta = zeusService.Perguntar(new Pergunta() { Descricao = pergunta });

                if (resposta.respondido)
                {
                    Console.WriteLine(resposta.resposta.Descricao);

                    "Sua pergunta foi respondida? sim(s) não(n)".Escreve();

                    var termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta, resposta.resposta.Pergunta);
                }
                else
                {
                    resposta.resposta.Descricao.Escreve();

                    var termo = Console.ReadLine();

                    if (termo.ToLower().Equals("s"))
                    {
                        "Diga agora:".Escreve();

                        termo = Console.ReadLine();
                        zeusService.Ensinar(new Resposta(termo), new Pergunta() { Descricao = pergunta });

                        "Obrigado por me responder! ;)".Escreve();
                    }
                }
            }
        }

        private static void PerguntaRespondidaInsatisfatoria(ZeusService zeusService, ref string termo, ref string pergunta, ref (Resposta resposta, bool respondido) resposta, Pergunta _pergunta = null)
        {
            if (!termo.ToLower().Equals("s"))
            {
                "Deseja obter mais respostas ou quer me ensinar? mais respostas(s) ensinar(e)".Escreve();
                termo = Console.ReadLine();

                if (termo.ToLower().Equals("s"))
                {
                    var respostas = zeusService.RespostasDeUmaPergunta(_pergunta);

                    if (respostas.Any())
                    {
                        
                        $"Achei aqui {respostas.Count} alternativas para a sua pergunta, escolha uma! alternativas (1,{respostas.Count}) continuar(s)".Escreve();
                       

                        var _termo = Console.ReadLine();

                        var list = new List<int>();
                        for (int i = 0; i < respostas.Count; i++) list.Add(i);

                        while (!_termo.ToLower().Equals("s") && list.Any(x => x == int.Parse(_termo)))
                        {
                            Console.WriteLine(respostas[int.Parse(_termo) - 1].Descricao);
                           
                            $"Deseja ver mais alternativas? alternativas (1,{respostas.Count}) continuar(s)".Escreve();

                            _termo = Console.ReadLine();
                        }
                    }

                    "Sua pergunta foi respondida? sim(s) não(n)".Escreve();

                    termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta);
                }
                else if (termo.ToLower().Equals("e"))
                {
                    "Ok, agora me diga como responder a sua pergunta! ;)".Escreve();

                    termo = Console.ReadLine();
                    zeusService.Ensinar(new Domain.Resposta(termo), new Domain.Pergunta() { Descricao = pergunta });

                    "Obrigado por me responder! ;)".Escreve();
                }
                else
                {
                    Console.Clear();

                    "Desculpe vamos tentar de novo! :(".Escreve();
                }
            }
        }
    }
}
