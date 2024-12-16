![site do pacote com A* pathfinder](https://arongranberg.com/astar/documentation/4_2_17_c030646a/)

# Desafio Individual Unity 3: Criando uma experiência de jogo

## Roadmap de Desenvolvimento

### Alta Prioridade (Essencial para o Desafio 3)
- [ ] **Tela de Menu de Entrada.**
- [ ] **Tela de Game Over com Animação.**
- [ ] **Sistema de Pause com Menu.**
- [ ] **IA Básica dos Inimigos (pathfinding para perseguição e movimentação em plataformas).**
- [ ] **Plantação de Sementes em Locais Específicos.**
- [ ] **Implementação de Frutos que Recuperam HP e Aumentam Dano.**

### Média Prioridade (Importante para o Desafio 3, mas pode ser simplificado)
- [ ] **Cutscene Inicial.**
- [ ] **Waves de Zumbis e Drop de Caixas de Munição.**
- [ ] **Drops Especiais (começar com 1 ou 2 itens, como "Modo Espectro").**
- [ ] **Sistema Básico de Ciclo Dia e Noite.**
### Baixa Prioridade (Menos Importante para o Desafio 3)
- [ ] **Zumbis Gigantes com Drops Especiais.**
- [ ] **Frutos com Efeitos Secundários (ex.: aumentar pulo, criar torretas).**
- [ ] **Sprites e Sons Temáticos (ex.: som do Metal Slug para o Modo Laser).**
- [ ] **Animações Avançadas no Ciclo de Dia e Noite.**

<div  align="center">

###### ATIVIDADE: DESAFIO INDIVIDUAL UNITY 3
###### SEMESTRE: 2024.2
###### DISCIPLINA: JOGOS DIGITAIS
###### CURSO: CIÊNCIA DA COMPUTAÇÃO
###### DOCENTE: LEANDRO MARQUES DO NASCIMENTO
###### DISCENTE: CHRISTIAN OLIVEIRA DO RAMO
###### MATRÍCULA: 200737470 

</div>


# Resumo

Esta atividade consiste na **continuação do DIU2** e no aprimoramento do jogo já desenvolvido, adicionando conceitos de **Game Design**.  
O DIU2 consistia em um pequeno jogo de sobrevivência (survival) com ambientação, tema e formato à sua escolha, seguindo um conjunto de regras básicas. Não havia restrições quanto à criação de um jogo 2D ou 3D.

---

## Objetivos do Desafio

Engajar o jogador em um jogo simples, porém divertido, criando uma experiência bem clara.  
O seu jogo deve responder à pergunta: **que experiência eu quero que meu jogador tenha?**

Você deve usar a criatividade para criar uma experiência rica no jogador, como foi apresentado nas vídeo-aulas.  
Um jogo pode ser muito simples e ainda assim apresentar uma experiência envolvente. Exemplos:

- **Space Invaders:** experiência de matar alienígenas em um jogo simples.
- **Escape the Room:** despertar da curiosidade/investigação.
- **The Sims:** cuidado com o próximo.
- **Fear:** medo/pavor.
- **GTA:** transgressão de regras.
- **Simuladores:** voar/pilotar.
- **Call of Duty:** participar de uma guerra.

O ponto principal deste desafio é transformar o “brinquedo” criado no DIU2 em um jogo, estimulando e desafiando o jogador a continuar jogando.  
Você está criando um **jogo casual**, que não precisa ser grande nem ter regras complexas para ser divertido e instigante.  
Exemplos de inspiração incluem jogos como **Pac-Man**, **Asteroids** e **Flappy Bird**.

### Exemplos de Jogos Simples e Casuais
- **Pular entre plataformas:** [Nice Jumper](http://bit.ly/nice_jumper)
- **Atirar e subir (Shoot 'em up):** [1942](http://bit.ly/shump-1942)
- **Plataforma 2D:** [Metal Slug](http://bit.ly/metal-slug-snk)

### Recursos e Ferramentas
- **Inspiração para projetos futuros:** [Unity Play](https://play.unity.com)
- **Tilemap para criação de cenários:** [Guia de Tilemap](https://bit.ly/2Qo8CVw)

---

## Regras Básicas do Jogo

1. Deve manter as regras do DIU2 (desafio cumulativo).
2. Deve ter **apenas uma fase**.
3. Deve incluir:
   - Menu de entrada.
   - Pause.
   - Mensagem de “game over”.
4. Implementar um **sistema básico de recompensas**:
   - Desafiar o jogador para ganhar algo ou punir falhas.
   - Exemplo: inimigos com comportamentos específicos e itens coletáveis.
5. Deve apresentar **movimentação especial de câmera** (ex.: [Cinemachine](https://bit.ly/33MpiJa)) **OU** comportamento inteligente do inimigo (ex.: [Pathfinding](https://bit.ly/2RjjuEx)).
6. Implementar uma ação especial do jogador que consuma recursos (ex.: dinheiro, vida, tempo), mas que traga clara vantagem.  
   - Exemplo: em jogos **Beat’em Up**, um golpe especial pode eliminar inimigos ao custo de um percentual de vida ([vídeo de exemplo](https://youtu.be/NZW7yzIrxOo?t=17)).

---

## Resultados Esperados

- **Vídeo curto** (até 10 minutos): apresentar o que foi desenvolvido, aprendido e dificuldades superadas.
- **Código do projeto Unity** (scripts e assets desenvolvidos) disponibilizado em repositório **GitHub**.
- **Jogo compatível com WebGL**, para execução pública ([guia WebGL](https://bit.ly/2Rjj2pP)).

---

## Pontuação da Atividade (máximo de 400 pontos)

- **50 pontos:** Menu de entrada, pause e mensagem de “game over”.
- **50 pontos:** Movimentação especial de câmera **OU** comportamento inteligente do inimigo.
- **50 pontos:** Ação especial do jogador que promova vantagem e penalidade.
- **100 pontos:** Sistema de recompensas que torne o jogo divertido e desafiador.
- **150 pontos:** Criatividade na produção de uma experiência divertida e desafiadora.

# Atualizações Planejadas para o Jogo

## Funcionalidades do Jogo

### Telas e Menus
- **Tela de Menu de Entrada:**
  - Interface inicial do jogo.

- **Tela de Game Over:**
  - Apresenta uma animação (inspirada em SOTN).
  - Após a animação, exibe um menu transparente avermelhado com as opções:
    - "Ir para o Menu".
    - "Tentar Novamente".

- **Sistema de Pause:**
  - Tela transparente azulada com as opções:
    - "Retomar".
    - "Menu".
    - "Tentar Novamente".

### Cutscenes
- **Cutscene Inicial:**
  - Mostra a trajetória da bandeira desde o ponto mais alto até onde o jogador está no início.

### Mecânicas de Jogo
- **Sobrevivência em Waves:**
  - Durante a noite, o jogador enfrenta waves de zumbis que emergem de portais.
  - Caixas de munição surgem de portais para coleta.

- **IA dos Inimigos:**
  - Zumbis perseguem o jogador e podem navegar por plataformas usando pathfinding.
  - Zumbis gigantes que atiram têm 50% de chance de dropar um item especial (probabilidade de 16,67% para cada drop).

- **Plantação de Sementes:**
  - Sementes só podem ser plantadas em locais específicos, sendo mais escassos em áreas elevadas.
  - O jogador precisa retornar para coletar o fruto único da planta, que desaparece após ser colhido.

- **Plantas e Frutos:**
  - Frutos podem gerar os seguintes efeitos:
    - Aumentar tamanho do HP.
    - Recuperar HP.
    - Aumentar dano do tiro.
    - Aumentar a cadência de tiros.
    - Fornecer tiro explosivo.
    - Permitir tiro multidirecional.
    - Gerar caixa de munição.
    - Aumentar altura do pulo.
    - Criar torreta temporária.
    - Aumentar velocidade de movimento.
    - Aumentar stamina de corrida.

### Drops Especiais
- Itens exclusivos dropados por zumbis gigantes:
  - **Modo Espectro:** Velocidade e altura de pulo extremas.
  - **Modo Invencível:** Jogador fica piscando e ganha música temática.
  - **Modo Laser:** Tiros que atravessam inimigos, com som e sprite inspirados em Metal Slug.

### Jogabilidade
- **Jogador:**
  - HP inicial: 100 (regenera apenas com frutos específicos).
  - Stamina inicial: 100 (regenera 10% por segundo após 2 segundos sem uso).
  - Corrida consome 10 pontos de stamina por segundo e é desativada ao esgotar.

- **Seleção de Sementes:**
  - Segure o botão direito do mouse para escolher a semente a plantar.

- **Ciclo Dia e Noite:**
  - Plantas brotam após um ciclo completo (dia + noite).
  - Rotação de sprites da lua e do sol indicam a passagem do tempo.

---

