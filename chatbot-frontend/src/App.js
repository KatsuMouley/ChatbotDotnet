import React, { useState, useRef, useEffect } from 'react';
import './App.css';

function App() {
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [userType, setUserType] = useState('user');
  const messagesEndRef = useRef(null);

  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages]);

  const sendMessage = () => {
    if (inputMessage.trim()) {
      const userMessage = { text: inputMessage, sender: userType };
      setMessages(prev => [...prev, userMessage]);

      // Verifica se o comando é "/toggle"
      if (inputMessage.startsWith('/toggle')) {
        setUserType(prevType => (prevType === 'user' ? 'admin' : 'user'));
        setInputMessage('');
        return;
      }

      // Verifica se o comando é "/image"
      if (inputMessage.startsWith('/image')) {
        const userInput = inputMessage.replace('/image', '').trim(); // Extrai o input do usuário após "/image"
        if (userInput) {
          fetch(`http://localhost:5228/api/Response/generate-image?prompt=${encodeURIComponent(userInput)}`)
            .then(response => {
              if (!response.ok) {
                throw new Error('Erro na resposta da API de imagem');
              }
              return response.blob(); // Recebe a imagem como Blob
            })
            .then(imageBlob => {
              const imageUrl = URL.createObjectURL(imageBlob); // Cria uma URL para exibir a imagem
              const botMessage = { text: imageUrl, sender: 'bot', isImage: true };
              setMessages(prev => [...prev, botMessage]);
            })
            .catch(error => {
              console.error('Erro ao obter a imagem:', error);
              const errorMessage = { text: 'Erro ao gerar a imagem.', sender: 'bot' };
              setMessages(prev => [...prev, errorMessage]);
            });
        }
        setInputMessage('');
        return;
      }

      // Caso não seja um comando "/toggle" ou "/image", faz a chamada padrão para o bot
      fetch('http://localhost:5228/api/Chat/ask', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Question: inputMessage }),
      })
        .then(response => {
          if (!response.ok) {
            throw new Error('Erro na resposta da API');
          }
          return response.json();
        })
        .then(data => {
          const botResponse = data.candidates[0].content.parts[0].text || "Resposta não recebida.";
          const botMessage = { text: botResponse, sender: 'bot' };
          setMessages(prev => [...prev, botMessage]);
        })
        .catch(error => {
          console.error('Erro:', error);
          const errorMessage = { text: 'Erro ao conectar com o bot.', sender: 'bot' };
          setMessages(prev => [...prev, errorMessage]);
        });

      setInputMessage('');
    }
  };

  return (
    <div className="chatbot-container">
      <div className="chatbot-header">
        <h2>Chat Bot</h2>
      </div>

      <div className="chatbot-messages">
        {messages.map((msg, index) => (
          <div key={index} className={`message ${msg.sender}`}>
            {msg.isImage ? (
              <img src={msg.text} alt="Generated" style={{ maxWidth: '100%' }} />
            ) : (
              msg.text
            )}
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>

      <div className="chatbot-input">
        <input
          type="text"
          placeholder="Digite sua mensagem..."
          value={inputMessage}
          onChange={(e) => setInputMessage(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && sendMessage()}
        />
        <button onClick={sendMessage}>Enviar</button>
      </div>
    </div>
  );
}

export default App;
