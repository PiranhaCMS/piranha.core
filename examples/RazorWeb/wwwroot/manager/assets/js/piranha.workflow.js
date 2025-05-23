window.piranha = window.piranha || {};
window.piranha.workflow = (function() {
  const apiBase = '/manager/api/workflow';

  // Verifica se o usuário é admin
  async function isAdmin() {
    const res = await fetch(`${apiBase}/isadmin`);
    const json = await res.json();
    return json.isAdmin;
  }

  // Badge de estado
  function stateBadge(state) {
    switch(state) {
      case 'PendingReview': return '<span class="badge badge-warning">Pendente</span>';
      case 'NeedsChanges':   return '<span class="badge badge-info">Precisa de alterações</span>';
      case 'Accepted':       return '<span class="badge badge-success">Aceito</span>';
      case 'Denied':         return '<span class="badge badge-danger">Rejeitado</span>';
      default:               return `<span class="badge badge-secondary">${state}</span>`;
    }
  }

  // Carrega conteúdo para admin (pendentes)
  async function loadAdmin() {
    const res   = await fetch(`${apiBase}/pending`);
    const items = await res.json();
    const tbody = document.querySelector('#workflowTable tbody');
    tbody.innerHTML = '';
    items.forEach(item => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${item.title}</td>
        <td>${item.contentType}</td>
        <td>${item.authorName || item.authorId}</td>
        <td>${stateBadge(item.state)}</td>
        <td>
          <button class="btn btn-sm btn-success" onclick="piranha.workflow.action('${item.id}','accept')"><i class="fas fa-check"></i></button>
          <button class="btn btn-sm btn-warning" onclick="piranha.workflow.action('${item.id}','changes')"><i class="fas fa-edit"></i></button>
          <button class="btn btn-sm btn-danger"  onclick="piranha.workflow.action('${item.id}','deny')"><i class="fas fa-times"></i></button>
        </td>`;
      tbody.appendChild(tr);
    });
  }

  // Carrega conteúdo para usuário normal (meus envios)
  async function loadUser() {
    const res   = await fetch(`${apiBase}/mine`);
    const items = await res.json();
    const tbody = document.querySelector('#workflowTable tbody');
    tbody.innerHTML = '';
    items.forEach(item => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${item.title}</td>
        <td>${item.contentType}</td>
        <td>${stateBadge(item.state)}</td>
        <td>${new Date(item.updated).toLocaleString('pt-PT')}</td>`;
      tbody.appendChild(tr);
    });
  }

  // Carrega conteúdos aprovados de outros
  async function loadOthers() {
    const res   = await fetch(`${apiBase}/approved`);
    const items = await res.json();
    const tbody = document.querySelector('#othersTable tbody');
    tbody.innerHTML = '';
    items.forEach(item => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${item.title}</td>
        <td>${item.contentType}</td>
        <td>${item.authorName || item.authorId}</td>
        <td>${new Date(item.published).toLocaleString('pt-PT')}</td>`;
      tbody.appendChild(tr);
    });
  }

  // Contador de pendentes no menu
  let lastCount = 0;
  async function updateCount() {
    const res = await fetch(`${apiBase}/count`);
    const { count } = await res.json();
    const badge = document.getElementById('workflowCount');
    if (badge) badge.textContent = count;
    if (count > lastCount) {
      piranha.notifications.push({
        type: 'info',
        body: `Você tem ${count - lastCount} nova(s) submissão(ões) pendente(s).`
      });
    }
    lastCount = count;
  }

  // Ações de admin: accept / deny / changes
  async function action(id, act) {
    const res = await fetch(`${apiBase}/${id}/${act}`, { method: 'POST' });
    if (res.ok) {
      piranha.notifications.push({ type: 'success', body: 'Ação realizada com sucesso.' });
      loadAdmin();
      updateCount();
    } else {
      piranha.notifications.push({ type: 'error',   body: 'Erro ao processar a ação.' });
    }
  }

  // Inicializadores
  function initAdminPage() {
    loadAdmin();
    updateCount();
    setInterval(updateCount, 30000);
  }

  function initUserPage() {
    loadUser();
    setInterval(loadUser, 30000);
  }

  function initOthersPage() {
    loadOthers();
  }

  return {
    isAdmin,
    initAdminPage,
    initUserPage,
    initOthersPage,
    action
  };
})();
