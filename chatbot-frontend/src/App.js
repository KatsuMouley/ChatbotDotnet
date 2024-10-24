import React, { useState, useRef, useEffect } from 'react';
import './App.css';

let messageId = 1;

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
      const userMessage = { id: Date.now(), text: inputMessage, sender: userType };
      setMessages(prev => [...prev, userMessage]);

      fetch('http://localhost:5228/api/Chat/InsertAsk', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Content: inputMessage }),
      });

      if (inputMessage.startsWith('/toggle')) {
        setUserType(prevType => (prevType === 'user' ? 'admin' : 'user'));
        setInputMessage('');
        return;
      }

      if (inputMessage.startsWith('/image')) {
        const userInput = inputMessage.replace('/image', '').trim();
        if (userInput) {
          fetch(`http://localhost:5228/api/Response/generate-image?prompt=${encodeURIComponent(userInput)}`)
            .then(response => {
              if (!response.ok) throw new Error('Erro na resposta da API de imagem');
              return response.blob();
            })
            .then(imageBlob => {
              const imageUrl = URL.createObjectURL(imageBlob);
              const botMessage = { id: Date.now(), text: imageUrl, sender: 'bot', isImage: true };
              setMessages(prev => [...prev, botMessage]);
            })
            .catch(error => {
              console.error('Erro ao obter a imagem:', error);
              const errorMessage = { id: Date.now(), text: 'Erro ao gerar a imagem.', sender: 'bot' };
              setMessages(prev => [...prev, errorMessage]);
            });
        }
        setInputMessage('');
        return;
      }

      if (inputMessage.startsWith('/reportAnswer')) {
        const answerId = inputMessage.split(' ')[1];
        if (answerId) {
          fetch(`http://localhost:5228/api/Chat/answers/${answerId}`)
            .then(response => {
              if (!response.ok) throw new Error('Erro ao buscar a resposta');
              return response.text();
            })
            .then(answerContent => {
              fetch('http://localhost:5228/api/Report', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ AnswerId: answerId, Message: answerContent }),
              })
                .then(response => {
                  if (!response.ok) throw new Error('Erro ao registrar a denúncia');
                  const reportMessage = { id: messageId++, text: `Denúncia registrada para a resposta de ID ${answerId}`, sender: 'bot' };
                  setMessages(prev => [...prev, reportMessage]);
                })
                .catch(error => {
                  console.error('Erro ao registrar a denúncia:', error);
                  const errorMessage = { id: messageId++, text: 'Erro ao registrar a denúncia.', sender: 'bot' };
                  setMessages(prev => [...prev, errorMessage]);
                });
            })
            .catch(error => {
              console.error('Erro ao buscar a resposta:', error);
              const errorMessage = { id: messageId++, text: 'Erro ao buscar a resposta.', sender: 'bot' };
              setMessages(prev => [...prev, errorMessage]);
            });
        }
        setInputMessage('');
        return;
      }

      fetch('http://localhost:5228/api/Chat/ask', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Question: inputMessage }),
      })
        .then(response => {
          if (!response.ok) throw new Error('Erro na resposta da API');
          return response.json();
        })
        .then(data => {
          const botResponse = data.candidates[0]?.content?.parts[0]?.text || "Resposta não recebida.";
          const botMessage = { id: Date.now(), text: botResponse, sender: 'bot' };
          setMessages(prev => [...prev, botMessage]);

          fetch('http://localhost:5228/api/Chat/InsertAnswer', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Content: botResponse }),
          });
        })
        .catch(error => {
          console.error('Erro:', error);
          const errorMessage = { id: Date.now(), text: 'Erro ao conectar com o bot.', sender: 'bot' };
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
        {messages.map((msg) => (
          <div key={msg.id} className={`message ${msg.sender}`}>
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
