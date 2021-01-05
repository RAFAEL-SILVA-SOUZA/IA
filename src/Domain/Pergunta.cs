using System.Collections.Generic;

namespace Zeus.Domain
{
    public class Pergunta
    {
        public Pergunta()
        {

            Respostas = new HashSet<Resposta>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public ICollection<Resposta> Respostas { get; set; }
    }
}
