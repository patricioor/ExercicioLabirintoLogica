# Exercício de Lógica - Labirinto Binário

O código fornecido é uma implementação de um algoritmo de resolução de labirinto que percorre uma matriz e tenta encontrar a saída, 
priorizando certas direções e não visitando células já visitadas. O algoritmo armazena o caminho percorrido em uma lista de strings chamada "resultado".

O algoritmo começa adicionando a posição atual (lAtual, cAtual) à lista "resultado" na forma de uma string que representa uma célula no labirinto, 
usando colchetes e as letras "O" para indicar a posição inicial. Em seguida, cria uma lista vazia chamada "visitado" e adiciona a posição inicial na forma de uma string a ela.

A variável "achouSaida" é definida como verdadeira se a posição atual for a mesma que a posição de saída (lSaida, cSaida).

O algoritmo usa um objeto de expressão regular chamado "regex" para extrair os inteiros das strings na lista "resultado". 
Ele também usa uma variável de contador chamada "contadorDeLoop" para acompanhar o número de loops que foram executados sem encontrar um caminho válido.

O algoritmo é executado em um loop while que continua até que a saída seja encontrada. Em cada iteração do loop, o algoritmo extrai a posição atual do último elemento da lista "resultado", 
usando o objeto de expressão regular e a variável de contador. Em seguida, verifica se a posição atual é a mesma que a posição de saída e, se for, o algoritmo define "achouSaida" como verdadeiro e sai do loop.

Em seguida, o algoritmo verifica as células vizinhas na seguinte ordem de prioridade: cima, esquerda, direita e baixo. Para cada direção, 
o algoritmo verifica se a célula não é uma parede (ou seja, se a célula contém "0" na matriz) e se ela ainda não foi visitada. Se uma célula válida for encontrada, 
o algoritmo adiciona a célula à lista "resultado" com o indicador de direção correspondente (C para cima, E para esquerda, D para direita e B para baixo) e adiciona a célula à lista "visitado".

O algoritmo também verifica se a célula em cada direção já foi visitada com o mesmo indicador de direção. Se já tiver sido visitada, o algoritmo não adiciona a célula à lista "resultado", 
para evitar ciclos no caminho.

Se o algoritmo não conseguir encontrar um caminho válido em nenhuma direção, incrementa o contador "contadorDeLoop" e tenta novamente, usando a posição anterior na lista "resultado". 
Isso garante que o algoritmo explore outras possíveis rotas antes de desistir.

Finalmente, o algoritmo imprime o último elemento da lista "resultado", que deve ser a posição de saída com o indicador de direção correspondente.

Exemplos de labirintos finalizados com o código:

![image](https://github.com/patricioor/ExercicioLabirintoLogica/assets/111129849/e0f636fd-9607-436c-b4cd-ff4d7307113e) ![image](https://github.com/patricioor/ExercicioLabirintoLogica/assets/111129849/5536693f-6060-4aec-805e-2387549d799e) ![image](https://github.com/patricioor/ExercicioLabirintoLogica/assets/111129849/bed1b246-3224-45d4-921d-7094d7799e99) ![image](https://github.com/patricioor/ExercicioLabirintoLogica/assets/111129849/ede4dbca-5d8c-4d5a-aaaf-657f3e82de3b)



