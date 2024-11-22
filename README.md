### **DIU1 - Desafio Individual Unity #1**
**Objetivo**: Desenvolver um personagem que se move pela tela, colide com objetos e executa ações com animações.

#### **Funcionalidades Requeridas e Adicionais**
1. **Movimentação do personagem**:
   - Implementação de movimentação em 2D ou 3D a partir de comandos do jogador (ex.: teclas de direção, WASD ou joystick).
   - Configuração de velocidades variáveis (ex.: andar e correr).
   - Detecção de colisões com:
     - Chão.
     - Paredes.
     - Outros objetos interativos (como plataformas móveis ou obstáculos).

2. **Comandos do jogador**:
   - Implementação de **pelo menos três comandos diferentes**, como:
     - Pular.
     - Correr.
     - Agachar.
     - Atirar um projétil.
     - Interagir com objetos.

3. **Animações**:
   - Implementação de animações básicas:
     - Animação de andar/correr.
     - Animação de pulo.
     - Animação de agachamento.
   - Animações avançadas:
     - Troca de estados suave entre animações.
     - Animação de impacto ao colidir.

4. **Cenário e objetos interativos**:
   - Criação de obstáculos variados (ex.: blocos, buracos, plataformas móveis).
   - Superfícies com propriedades específicas (ex.: áreas deslizantes ou de impulso).

5. **Extras para pontuação máxima**:
   - Sistema de partículas simples (ex.: poeira ao correr, faíscas ao colidir).
   - Feedback visual ou sonoro ao executar ações (ex.: som de passos, barulho ao colidir).
   - Controle configurável (ex.: suporte a teclado e controle de console).

---

### **DIU2 - Desafio Individual Unity #2**
**Objetivo**: Expandir o jogo anterior para um jogo de sobrevivência com inimigos e mecânicas de dificuldade progressiva.

#### **Funcionalidades Requeridas e Adicionais**
1. **Personagem principal**:
   - Herança de todas as funcionalidades do **DIU1**.
   - Pelo menos **três ações principais**:
     - Pular.
     - Atirar projéteis.
     - Esquivar ou realizar ataques físicos.
   - Animações adicionais:
     - Animação de ataque/tiro.
     - Animação de morte.

2. **Inimigos**:
   - Pelo menos **um tipo de inimigo**, com:
     - Comportamento básico (ex.: seguir o jogador, patrulhar áreas).
     - Animação ao se mover e ao atacar.
   - Sistema de "morte" do inimigo:
     - Remoção do inimigo da tela ao ser atacado.
     - Respawn do inimigo após determinado tempo ou em local aleatório.

3. **Mecânica de dificuldade progressiva**:
   - Inimigos surgem gradualmente, aumentando:
     - Velocidade de movimento.
     - Quantidade ou frequência de surgimento.
   - Introdução de novos obstáculos (ex.: plataformas desaparecendo, pedras caindo).

4. **Morte e reinício**:
   - Implementação do estado de morte do personagem:
     - Reset do jogo ao início (sem mensagem de game over).
     - Feedback visual ao morrer (ex.: animação ou efeitos sonoros).

5. **Ambientação e cenário**:
   - Criação de um cenário básico (ex.: floresta, deserto, ruínas).
   - Elementos interativos no cenário (ex.: barris explosivos, plataformas móveis).

6. **Extras para pontuação máxima**:
   - Variação nos tipos de inimigos (ex.: inimigos voadores, projéteis móveis).
   - Colecionáveis simples (ex.: moedas, vida extra).
   - Sistema de partículas avançado (ex.: explosões, faíscas).
   - Sons e músicas temáticas.

---

### **DIU3 - Desafio Individual Unity #3**
**Objetivo**: Refinar o jogo anterior, adicionando menus, sistema de recompensas, movimentação de câmera e ações especiais.

#### **Funcionalidades Requeridas e Adicionais**
1. **Menus e interface**:
   - Menu principal:
     - Opções: "Jogar", "Configurações", "Sair".
   - Menu de pause:
     - Opções: "Continuar", "Reiniciar", "Sair".
   - Tela de game over com feedback para o jogador.

2. **Recompensas e penalidades**:
   - Implementação de um sistema de recompensas:
     - Itens coletáveis (ex.: moedas, vidas extras).
     - Pontuação ou bônus por eliminar inimigos ou sobreviver mais tempo.
   - Penalidades claras:
     - Perda de vida ou recursos ao ser atingido.
     - Redução de pontos se uma tarefa não for concluída.

3. **Movimentação especial de câmera**:
   - Uso de Cinemachine para:
     - Transições suaves entre áreas.
     - Zoom in/out em momentos de ação.
     - Foco em inimigos ou eventos específicos.

4. **Ação especial do jogador**:
   - Habilidade poderosa que consome recursos:
     - Exemplo: Explosão que elimina todos os inimigos próximos em troca de vida ou mana.
     - Feedback visual (ex.: flash de luz, ondas de choque).
   - Interface para exibir recursos consumíveis (ex.: barra de mana/vida).

5. **Inimigos inteligentes**:
   - Comportamento avançado:
     - Pathfinding (movimento em torno de obstáculos).
     - Reação ao jogador (ex.: se esconder ou atacar em grupo).
   - Variação de inimigos:
     - Tipos com ataques e movimentações distintas.

6. **Ambientação e cenário**:
   - Cenário detalhado com elementos dinâmicos:
     - Objetos destrutíveis (ex.: caixas, barris).
     - Áreas interativas (ex.: trampolins, armadilhas).
   - Feedback visual (ex.: iluminação dinâmica, partículas ambientais).

7. **Extras para pontuação máxima**:
   - Power-ups temporários (ex.: invulnerabilidade, dano extra).
   - Transições cinematográficas para introduzir inimigos ou eventos.
   - Sons e músicas dinâmicas que reagem à ação do jogador.