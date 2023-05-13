using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atividade
{
    public partial class frmAtividade : Form
    {
        public frmAtividade()
        {
            InitializeComponent();
        }



        private void btnRun_Click(object sender, EventArgs e)
        {
            if (txtArquivo.Text.Trim().Equals(""))
            {
                MessageBox.Show(this, "Caminho do arquivo deve ser informado");
                txtArquivo.Focus();
                return;
            }

            if (!File.Exists(txtArquivo.Text.Trim()))
            {
                MessageBox.Show(this, "Arquivo inexistente!");
                txtArquivo.Focus();
                return;
            }

            Thread thread = new Thread(() => ExecutaAtividade(txtArquivo.Text.Trim()));
            thread.Name = "Atividade - Run";
            thread.Start();
        }


        private void ExecutaAtividade(string filePath)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                txtArquivo.Enabled = false;
                btnRun.Enabled = false;
            }));

            try
            {
                CodigoAtividade(filePath);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show(this, "Finalizado!");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show(this, ex.Message);
                }));
            }
            finally
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    txtArquivo.Enabled = true;
                    btnRun.Enabled = true;
                }));
            }
        }

        private void CodigoAtividade(string filePath)
        {
            // Lê o arquivo de entrada e o converte num array de string
            string[] lines = File.ReadAllLines(filePath);

            // Lê a primeira linha, quebrando ela em informações separadas pelo espaço
            string[] firstLine = lines[0].Split(' ');

            // Primeira informação é a quantidade de linhas
            int linhas = Convert.ToInt32(firstLine[0]);
            // Segunda informação é a quantidade de colunas
            int colunas = Convert.ToInt32(firstLine[1]);

            // Preenche matriz do labirinto
            string[,] matriz = new string[linhas, colunas];
            int lAtual = -1; // Posição inicial: linha
            int cAtual = -1; // Posição inicial: coluna
            int lSaida = -1; // Saída: linha
            int cSaida = -1; // Saída: coluna

            // percorre toda a matriz (a partir da segunda linha do arquivo texto) para identificar a posição inicial e a saída
            for (int l = 1; l < lines.Length; l++)
            {
                string[] line = lines[l].Split(' ');
                for (int c = 0; c < line.Length; c++)
                {
                    string ll = line[c];
                    matriz[l - 1, c] = ll;

                    if (ll.Equals("X"))
                    {
                        // Posição inicial
                        lAtual = l - 1;
                        cAtual = c;
                    }
                    else if (ll.Equals("0") && (l == 1 || c == 0 || l == lines.Length - 1 || c == line.Length - 1))
                    {
                        // Saída
                        lSaida = l - 1;
                        cSaida = c;
                    }
                }
            }

            // Posição máxima de linha e coluna que pode ser movida (borda)
            int extremidadeLinha = linhas - 1;
            int extremidadeColuna = colunas - 1;

            // Guarda o trajeto em uma list de string e já inicia com a posição de origem
            List<string> resultado = new List<string>();
            resultado.Add("O [" + (lAtual + 1) + ", " + (cAtual + 1) + "]");
            List<string> visitado = new List<string>();
            visitado.Add($"{lAtual + 1}{cAtual + 1}");

            // Percorre a matriz (labirinto) até encontrar a saída, usando as regras de prioridade e posições não visitadas, e vai armazenando o trajeto na list resultado
            bool achouSaida = lAtual == lSaida && cAtual == cSaida;

            //Instancia um objeto "regex" com a expressão regular "@"\d+" fará o regex receber apenas números.
            //O uso do regex tbm é para possibilitar o uso do tipo MatchCollection, e ter acesso ao método "Matches()"
            Regex regex = new Regex("[0-9]+");

            //Variável que irá contar os loops que serão incrementados caso as coordenadas não viabilizem nenhuma rota.
            var contadorDeLoop = 0;

            while (!achouSaida)
            {
                //Matches recebe elemento contido no resultado com base no tamanho da lista(resultado.Count) e decrementando "1" para adequar ao índice máximo da lista.
                //Em conjunto é decrementando o valor contido em "contadorDoLoop" para retornar alguns elementos e procurar novas alternativas de rota.
                MatchCollection matches = regex.Matches(resultado[(resultado.Count - 1 < 0 ? 0 : resultado.Count - 1) - contadorDeLoop]);

                // matches é utilizado para pegar o elementos do tipo "int" separdos pelo regex
                lAtual = int.Parse(matches[0].Value);
                cAtual = int.Parse(matches[1].Value);
                Console.WriteLine($"{resultado.Last()[0]} ({lAtual}, {cAtual})");

                // Achou a saída?
                if (lAtual - 1 == lSaida && cAtual - 1 == cSaida)
                {
                    achouSaida = true;
                    break;
                }

                // Direcionais que convertem as coordenadas da lista "resultado", outrora acrescidos +1,+1 para adaptar ao modelo da matriz que não começa do índice 0,
                // em pontos que sejam correspondendo na matriz.

                // Cima
                // Converter o resultado (-1,-1) para célula da matriz e adicionar (-1,0)
                //(-1,-1) + ( -1, 0) = (-2, -1)
                bool subir = matriz[lAtual - 2 > 0 ? lAtual - 2 : 0, cAtual - 1 > 0 ? cAtual - 1 : 0] != "0";

                // Esquerda
                // Converter o resultado (-1,-1) para célula da matriz e adicionar (0,-1).
                //(-1,-1) + ( 0, -1) = (-1, -2)
                bool esquerda = matriz[lAtual - 1 > 0 ? lAtual - 1 : 0, cAtual - 2 > 0 ? cAtual - 2 : 0] != "0";

                // Direita
                // Converter o resultado (-1,-1) para célula da matriz e adicionar (0,+1).
                //(-1,-1) + ( 0, +1) = (-1, 0)
                bool direita = matriz[lAtual - 1 > 0 ? lAtual - 1 : 0, cAtual] != "0";

                // Baixo
                // Converter o resultado (-1,-1) para célula da matriz e adicionar (+1,0).
                // (-1, -1) + (+1, 0) = (0, -1)
                bool descer = matriz[lAtual, cAtual - 1 > 0 ? cAtual - 1 : 0] != "0";

                // Verificar se a posição destino já foi registrada com a mesma direção
                // Célula já registrada com o direcional "C"(subir)?
                bool jaEsteveCima = resultado.Contains("C [" + (lAtual - 1 > 0 ? lAtual - 1 : 0) + ", " + (cAtual) + "]");

                // Célula já registrada com o direcional "E"(Esquerda)?
                bool jaEsteveEsquerda = resultado.Contains("E [" + (lAtual) + ", " + (cAtual - 1) + "]");

                // Célula já registrada com o direcional "D"(Direita)?
                bool jaEsteveDireita = resultado.Contains("D [" + (lAtual) + ", " + (cAtual + 1 > extremidadeColuna ? extremidadeColuna: cAtual + 1) + "]");

                // Célula já registrada com o direcional "B"(Descer)?
                bool jaEsteveBaixo = resultado.Contains("B [" + (lAtual + 1 > extremidadeLinha ? extremidadeLinha: lAtual +1) + ", " + (cAtual) + "]");

                //Variável "foiVisitado" indicará com "true" se a célula já foi registrada independente do direcional ou retornará "false" caso contrário.
                bool foiVisitadoCima = visitado.Contains($"{(lAtual - 1 > 0 ? lAtual - 1 : 0)}{cAtual}");

                bool foiVisitadoEsquerda = visitado.Contains($"{lAtual}{(cAtual - 1 > 0 ? cAtual - 1 : 0)}");

                bool foiVisitadoDireita = visitado.Contains($"{lAtual}{(cAtual + 1 > extremidadeColuna ? extremidadeColuna: cAtual + 1)}");

                bool foiVisitadoBaixo = visitado.Contains($"{(lAtual + 1 > extremidadeLinha ? extremidadeLinha: lAtual +1)}{cAtual}");

                //Converte a variável "foiVisitado" caso 3 das 4 possibilidades de movimentação sejam blocos inacessíveis

                if (esquerda && direita && descer)
                    foiVisitadoCima = false;

                if (subir && direita && descer)
                    foiVisitadoEsquerda = false;

                if (subir && esquerda && descer)
                    foiVisitadoDireita = false;

                if(subir && direita && esquerda)
                    foiVisitadoBaixo = false;

                // Variável "prioridade" vai verificar se a célula é propria para registro ou se ela já foi visitada.

                bool prioridadeCima = (esquerda ^ foiVisitadoEsquerda) && (direita ^ foiVisitadoDireita) && (descer ^ foiVisitadoBaixo);

                bool prioridadeEsquerda = (subir ^ foiVisitadoCima) && (direita ^ foiVisitadoDireita) && (descer ^ foiVisitadoBaixo);

                bool prioridadeDireita = (subir ^ foiVisitadoCima) && (esquerda ^ foiVisitadoEsquerda) && (descer ^ foiVisitadoBaixo);

                bool prioridadeBaixo = (subir ^ foiVisitadoCima) && (direita ^ foiVisitadoDireita) && (esquerda ^ foiVisitadoEsquerda);

                //Lógica final para o if
                bool podeSubir = !subir && !jaEsteveCima;

                bool podeEsquerda = !esquerda && !jaEsteveEsquerda;

                bool podeDireita = !direita && !jaEsteveDireita;

                bool podeDescer = !descer && !jaEsteveBaixo;

                //Confirmar que o caminho que será priorizado não é uma parede.
                if (prioridadeCima && !podeSubir) prioridadeCima = false;
                if (prioridadeEsquerda && !podeEsquerda) prioridadeEsquerda = false;
                if (prioridadeDireita && !podeDireita) prioridadeDireita = false;
                if (prioridadeBaixo && !podeDescer) prioridadeBaixo = false;

                //Filtro para priorizar células que levarão a novas rotas

                if (prioridadeCima)
                {
                    podeEsquerda = false;
                    podeDireita = false;
                    podeDescer = false;
                }

                if (prioridadeEsquerda)
                {
                    podeSubir = false;
                    podeDireita = false;
                    podeDescer =false;
                }

                if (prioridadeDireita)
                {
                    podeSubir = false;
                    podeEsquerda = false;
                    podeDescer = false;
                }

                if(prioridadeBaixo)
                {
                    podeSubir = false;
                    podeEsquerda = false;
                    podeDireita = false;
                }

                // Condição só permite a entrada se além da condição do "podeSubir","podeEsquerda", "podeDireita" e "podeDescer" ser atendida,
                // não pode ter registrado o mesmo ponto na mesma direção.
                // Caso alguma condição seja atendida, será zerado o contadorDeLoop.
                if (podeSubir)
                {
                    resultado.Add("C [" + (lAtual - 1) + ", " + (cAtual) + "]");
                    visitado.Add($"{lAtual - 1}{cAtual}");
                    contadorDeLoop = 0;
                }
                else if (podeEsquerda)
                {
                    resultado.Add("E [" + (lAtual) + ", " + (cAtual - 1) + "]");
                    visitado.Add($"{lAtual}{cAtual - 1}");
                    contadorDeLoop = 0;
                }
                else if (podeDireita)
                {
                    resultado.Add("D [" + (lAtual) + ", " + (cAtual + 1) + "]");
                    visitado.Add($"{lAtual}{cAtual + 1}");
                    contadorDeLoop = 0;
                }
                else if (podeDescer)
                {
                    resultado.Add("B [" + (lAtual + 1) + ", " + (cAtual) + "]");
                    visitado.Add($"{lAtual + 1}{cAtual}");
                    contadorDeLoop = 0;
                }

                //Caso não seja encontrada nenhuma combinação, o contador será incrementado.
                else
                {
                    contadorDeLoop++;
                }
            }
            // Salva arquivo texto de saída com o trajeto
            string folderPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            File.WriteAllLines(Path.Combine(folderPath, "saida-" + fileName + ".txt"), resultado.ToArray(), Encoding.UTF8);
        }
    }
}

