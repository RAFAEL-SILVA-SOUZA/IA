namespace Zeus.Domain
{
    public class Resposta
    {

        public Resposta(string descricao)
        {
            Descricao = descricao;
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public int PerguntaId { get; set; }
        public virtual Pergunta Pergunta { get; set; }

    }
}
