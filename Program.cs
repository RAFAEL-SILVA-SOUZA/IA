using System;
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

                Console.WriteLine("Como posso ajudá-lo agora?");
                var pergunta = Console.ReadLine();

                var resposta = zeusService.Perguntar(new Domain.Pergunta() { Descricao = pergunta });

                if (resposta.respondido)
                {
                    Console.WriteLine(resposta.resposta.Descricao);
                    Console.WriteLine("Sua pergunta foi respondida? sim(s) não(n)");
                    var termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta);
                }
                else
                {
                    Console.WriteLine(resposta.resposta.Descricao);
                    var termo = Console.ReadLine();

                    if (termo.ToLower().Equals("s"))
                    {
                        Console.WriteLine("Diga agora:");
                        termo = Console.ReadLine();
                        zeusService.Ensinar(new Domain.Resposta(termo), new Domain.Pergunta() { Descricao = pergunta });
                        Console.WriteLine("Obrigado por me responder! ;)");
                    }
                }
            }
        }

        private static void PerguntaRespondidaInsatisfatoria(ZeusService zeusService, ref string termo, ref string pergunta, ref (Domain.Resposta resposta, bool respondido) resposta)
        {
            if (!termo.ToLower().Equals("s"))
            {
                Console.WriteLine("Deseja obter mais respostas ou quer me ensinar? mais respostas(s) ensinar(e)");
                termo = Console.ReadLine();

                if (termo.ToLower().Equals("s"))
                {
                    resposta = zeusService.Perguntar(new Domain.Pergunta() { Descricao = pergunta });
                    Console.WriteLine(resposta.resposta.Descricao);
                    Console.WriteLine("Sua pergunta foi respondida? sim(s) não(n)");
                    termo = Console.ReadLine();
                    PerguntaRespondidaInsatisfatoria(zeusService, ref termo, ref pergunta, ref resposta);
                }
                else if (termo.ToLower().Equals("e"))
                {
                    Console.WriteLine("Ok, agora me diga como responder a sua pergunta! ;)");
                    termo = Console.ReadLine();
                    zeusService.Ensinar(new Domain.Resposta(termo), new Domain.Pergunta() { Descricao = pergunta });
                    Console.WriteLine("Obrigado por me responder! ;)");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Desculpe vamos tentar de novo! :(");
                }
            }
        }
    }
}
