using System;
using System.Collections.Generic;
using System.Linq;
using Zeus.Domain;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Como posso ajudá-lo agora?");
                Console.ForegroundColor = ConsoleColor.White;
                var pergunta = Console.ReadLine();

                var resposta = zeusService.Perguntar(new Pergunta() { Descricao = pergunta });

                if (resposta.respondido)
                {                    
                    Console.WriteLine(resposta.resposta.Descricao);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Sua pergunta foi respondida? sim(s) não(n)");
                    Console.ForegroundColor = ConsoleColor.White;
                    var termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta, resposta.resposta.Pergunta);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(resposta.resposta.Descricao);
                    Console.ForegroundColor = ConsoleColor.White;
                    var termo = Console.ReadLine();

                    if (termo.ToLower().Equals("s"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Diga agora:");
                        Console.ForegroundColor = ConsoleColor.White;
                        termo = Console.ReadLine();
                        zeusService.Ensinar(new Resposta(termo), new Pergunta() { Descricao = pergunta });
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Obrigado por me responder! ;)");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        private static void PerguntaRespondidaInsatisfatoria(ZeusService zeusService, ref string termo, ref string pergunta, ref (Resposta resposta, bool respondido) resposta, Pergunta _pergunta = null)
        {
            if (!termo.ToLower().Equals("s"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Deseja obter mais respostas ou quer me ensinar? mais respostas(s) ensinar(e)");
                Console.ForegroundColor = ConsoleColor.White;
                termo = Console.ReadLine();
                
                if (termo.ToLower().Equals("s"))
                {
                    var respostas = zeusService.RespostasDeUmaPergunta(_pergunta);

                    if (respostas.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Achei aqui {respostas.Count} alternativas para a sua pergunta, escolha uma! alternativas (1,{respostas.Count}) continuar(s)");
                        Console.ForegroundColor = ConsoleColor.White;
                        var _termo = Console.ReadLine();

                        var list = new List<int>();
                        for (int i = 0; i < respostas.Count; i++) list.Add(i);

                        while (!_termo.ToLower().Equals("s") && list.Any(x => x == int.Parse(_termo)))
                        {
                            Console.WriteLine(respostas[int.Parse(_termo) - 1].Descricao);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Deseja ver mais alternativas? alternativas (1,{respostas.Count}) continuar(s)");
                            Console.ForegroundColor = ConsoleColor.White;
                            _termo = Console.ReadLine();
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Sua pergunta foi respondida? sim(s) não(n)");
                    Console.ForegroundColor = ConsoleColor.White;
                    termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta);
                }
                else if (termo.ToLower().Equals("e"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ok, agora me diga como responder a sua pergunta! ;)");
                    Console.ForegroundColor = ConsoleColor.White;
                    termo = Console.ReadLine();
                    zeusService.Ensinar(new Domain.Resposta(termo), new Domain.Pergunta() { Descricao = pergunta });
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Obrigado por me responder! ;)");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Desculpe vamos tentar de novo! :(");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
