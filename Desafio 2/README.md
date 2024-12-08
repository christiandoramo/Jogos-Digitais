# Desafio 2

<div  align="center">

###### ATIVIDADE: DESAFIO INDIVIDUAL UNITY 2
###### SEMESTRE: 2024.2
###### DISCIPLINA: JOGOS DIGITAIS
###### CURSO: CIÊNCIA DA COMPUTAÇÃO
###### DOCENTE: LEANDRO MARQUES DO NASCIMENTO
###### DISCENTE: CHRISTIAN OLIVEIRA DO RAMO
###### MATRÍCULA: 200737470 

</div>

# Desafio Individual Unity 2: Estudo Dirigido em C#

## Resumo
Esta atividade consiste na criação de um pequeno jogo de sobrevivência (survival) com ambientação, tema e formato à sua escolha, mas seguindo um conjunto de **regras básicas**. Jogos de sobrevivência normalmente não apresentam fim.

### Dica
Este jogo será usado como base no próximo desafio, então escolha bem um tema/estilo de jogo para que seja interessante também para o próximo desafio.

---

## Objetivos do Desafio
1. **Entender a sintaxe básica de C# e suas particularidades no ambiente Unity.**
2. **Criar um jogo com um objetivo simples e explícito.** Exemplos de games:
   - [Nice Jumper](http://bit.ly/nice_jumper): Pular entre plataformas.
   - [Shoot 'em up](http://bit.ly/shump-1942): Atirar e subir.
   - [2D Platformer and Shooter](http://bit.ly/metal-slug-snk): Plataforma 2D.
3. **Observar outros jogos como inspiração para projetos futuros:** [Unity Play](https://play.unity.com).

---

## Regras Básicas do Jogo
- O jogo deve ter um **personagem principal** com pelo menos **3 ações** (ex.: pular, correr, atirar).
- Deve haver pelo menos **1 tipo de inimigo** com comportamento e **1 animação**. O personagem precisa de **2 animações**.
- O inimigo pode atacar o personagem, por exemplo:
  - Colisão (como uma pedra que se move e atinge o personagem).
  - Arremesso de projétil.
- Se atingido, o personagem deve "morrer" e reiniciar o jogo. Não é necessário incluir telas de *Game Over*.
- O personagem pode atacar inimigos para removê-los do cenário.
- Implementar **respawn** dos inimigos ao serem derrotados.
- O jogo deve aumentar a dificuldade gradativamente. Exemplos:
  - Reduzir o número de plataformas ou torná-las menores.
  - Aumentar a frequência de inimigos.

---

## Resultados Esperados
1. **Vídeo curto (até 10 minutos):**
   - Apresente o que foi desenvolvido.
   - Compartilhe aprendizados e dificuldades superadas.
2. **Código no GitHub:**
   - Disponibilize o projeto Unity com scripts e *assets* criados.

---

## Pontuação (máximo de 300 pontos)
- **50 pontos**: Animações do personagem e inimigo.
- **75 pontos**: Implementação da regra de "morte" e reinício do personagem.
- **75 pontos**: Implementação da regra de "morte" e *respawn* do inimigo.
- **100 pontos**: Criatividade para tornar o jogo envolvente.

---

## Referências
- **[Intermediate Gameplay Scripting (Unity Learn)](https://learn.unity.com/project/intermediate-gameplay-scripting)**
- **[2D Platformer Template (Unity Learn)](https://learn.unity.com/project/2d-platformer-template)**
- **[Introdução à Programação com Unity (Coursera)](https://www.coursera.org/learn/introduction-programming-unity)**
- **[Clone de Mega Man (Unity)](http://bit.ly/clone-megaman-unity)**
- **[Curso Básico de Introdução à Unity (Udemy)](https://www.udemy.com/course/curso-basico-de-introducao-a-unity/)**
- **[Unity Essentials (Unity Learn)](https://learn.unity.com/pathway/unity-essentials)**
- **Vídeos no YouTube:**
  - [Criando um Jogo em 2D](https://youtu.be/on9nwbZngyw)
  - [Movimentação em Unity 2D](https://youtu.be/dwcT-Dch0bA)
  - [Animação 2D - Sprite Sheet Animation](https://youtu.be/hkaysu1Z-N8)
- **Playlist em PT-BR:** [Criando um Jogo 2D na Unity](http://bit.ly/unity-2dplatformer)
- **Assets:** [2D Mega Pack](https://devassets.com/assets/2d-mega-pack/)

---

## Guia de Estudo – Sintaxe Básica de C#
### Conceitos Fundamentais
- **Classes e Objetos**
  - Tipos primitivos e de referência.
  - Instanciação e manipulação de objetos.
  - Atributos, construtores e métodos.
  - Conversões entre tipos e *casts*.
- **Arrays e Métodos Estáticos**
  - Métodos e atributos estáticos.
  - Uso de *namespaces*.
- **Conceitos de OO**
  - Encapsulamento, polimorfismo, *overloading*, *overriding*.
  - Ligação dinâmica (*dynamic binding*).
- **Estruturas de Controle de Fluxo**
  - `if/else`, `while`, `do-while`, `for`, `switch`.
- **Herança e Subclasses**
  - Sobrescrita, polimorfismo e transformação entre tipos.
  - Classes abstratas e interfaces.
- **Tratamento de Exceções**
  - Manipulação e uso correto de exceções.

### APIs Necessárias
1. **Coleções**:
   - Listas (*List*), conjuntos (*Set*), mapas (*Map*).
2. **Threading**:
   - Processamento paralelo, simultaneidade e programação assíncrona.
