'use strict'

const chatWindow = document.getElementById('chat-window');
const iconOpen = document.getElementById('icon-open');
const iconClose = document.getElementById('icon-close');
const messagesContainer = document.getElementById('chat-messages');
const userInput = document.getElementById('user-input');
const loadingIndicator = document.getElementById('chat-loading');

function toggleChat() {
    chatWindow.classList.toggle('hidden');
    chatWindow.classList.toggle('flex');

    if (chatWindow.classList.contains('hidden')) {
        iconOpen.classList.remove('hidden');
        iconClose.classList.add('hidden');
    } else {
        iconOpen.classList.add('hidden');
        iconClose.classList.remove('hidden');
        setTimeout(() => userInput.focus(), 100);
    }
}

async function sendMessage(e) {
    e.preventDefault();
    const message = userInput.value.trim();
    if (!message) return;

    appendMessage('User', message);
    userInput.value = '';

    loadingIndicator.classList.remove('hidden');
    messagesContainer.scrollTop = messagesContainer.scrollHeight;

    try {
        const response = await fetch('/api/ai/support', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({ prompt: message })
        });

        if (!response.ok) throw new Error('Network response was not ok');

        const data = await response.json();

        console.log(data);

        loadingIndicator.classList.add('hidden');
        appendMessage('AI', data.answer || "I received your message.");

    } catch (error) {
        console.error('Error:', error);
        loadingIndicator.classList.add('hidden');
        appendMessage('AI', "Sorry, I'm having trouble connecting right now.");
    }
}

function appendMessage(sender, text) {
    const isUser = sender === 'User';

    const messageDiv = document.createElement('div');
    messageDiv.className = `flex items-start gap-2.5 ${isUser ? 'flex-row-reverse' : ''}`;

    const avatarHtml = isUser
        ? `<div class="w-8 h-8 rounded-full bg-gray-200 flex items-center justify-center text-xs font-bold text-gray-600">You</div>`
        : `<img src="/images/buddy.png" class="w-8 h-8 rounded-full" alt="sara" />`;

    const bubbleColor = isUser ? 'bg-blue-600 text-white rounded-s-xl rounded-ee-xl' : 'bg-white text-gray-900 rounded-e-xl rounded-es-xl border border-gray-200 shadow-sm';

    let formattedText = text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");

    formattedText = formattedText.replace(/\n/g, '<br>');

    const linkStyle = isUser ? 'text-white underline' : 'text-blue-600 hover:underline font-semibold';

    formattedText = formattedText.replace(
        /\[([^\]]+)\]\(([^)]+)\)/g,
        `<a href="$2" target="_blank" class="${linkStyle}">$1</a>`
    );

    formattedText = formattedText.replace(/\*\*(.*?)\*\*/g, '<span class="font-bold">$1</span>');

    messageDiv.innerHTML = `
                ${avatarHtml}
                <div class="flex flex-col gap-1 w-full max-w-[320px]">
                    <div class="flex flex-col leading-1.5 p-4 ${bubbleColor}">
                        <p class="text-sm font-normal">${formattedText}</p>
                    </div>
                </div>
            `;

    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}