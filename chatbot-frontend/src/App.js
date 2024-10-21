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

      if (inputMessage.startsWith('/toggle')) {
        setUserType(prevType => (prevType === 'user' ? 'admin' : 'user'));
        setInputMessage('');
        return;
      } else if (inputMessage.startsWith('/teach')) {
        const teachingText = inputMessage.slice(7).trim();
        fetch('http://localhost:5228/api/Response/teach', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Keyword: teachingText, Text: 'Nova resposta', Variant: 'default' }),
        })
        .then(response => response.json())
        .then(() => {
          const botMessage = { text: 'Nova resposta adicionada para aprovação.', sender: 'bot' };
          setMessages(prev => [...prev, botMessage]);
        })
        .catch(error => console.error('Erro:', error));
        setInputMessage('');
        return;
      } else if (inputMessage.startsWith('/report')) {
        const messageId = inputMessage.slice(8).trim();
        fetch('http://localhost:5228/api/Response/report', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ MessageId: messageId }),
        })
        .then(() => {
          const botMessage = { text: 'Mensagem reportada.', sender: 'bot' };
          setMessages(prev => [...prev, botMessage]);
        })
        .catch(error => console.error('Erro:', error));
        setInputMessage('');
        return;
      } else if (inputMessage.startsWith('/add variant')) {
        const variantText = inputMessage.slice(12).trim();
        fetch('http://localhost:5228/api/Response/addVariant', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Variant: variantText }),
        })
        .then(() => {
          const botMessage = { text: `Variante "${variantText}" adicionada.`, sender: 'bot' };
          setMessages(prev => [...prev, botMessage]);
        })
        .catch(error => console.error('Erro:', error));
        setInputMessage('');
        return;
      } else if (inputMessage.startsWith('/image')) {
        const imageName = inputMessage.slice(7).trim(); // Get the image name
        fetch(`http://localhost:5228/api/Response/image/${imageName}`, { // Adjusted URL
            method: 'GET', // Changed to GET, assuming your endpoint uses GET for images
            headers: { 'Content-Type': 'application/json' },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Image not found');
            }
            return response.blob(); // Use .blob() to handle the binary image data
        })
        .then(imageBlob => {
            const imageUrl = URL.createObjectURL(imageBlob); // Create a URL for the image blob
            const botMessage = { text: imageUrl, sender: 'bot', isImage: true }; // Add `isImage`
            setMessages(prev => [...prev, botMessage]);
        })
        .catch(error => console.error('Erro:', error));
        setInputMessage('');
        return;
    }
    

      fetch('http://localhost:5228/api/Response/chat', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ text: inputMessage }), // Alterado para enviar um objeto
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro na resposta da API');
        }
        return response.json();
    })
    .then(data => {
        const botMessage = { text: data.text || "Resposta não recebida.", sender: 'bot' };
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
            {msg.isImage ? <img src={msg.text} alt="Generated" style={{ maxWidth: '100%' }} /> : msg.text}
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
