(() => {
  const toggle = document.getElementById('menuToggle');
  const sidebar = document.getElementById('sidebar');
  toggle?.addEventListener('click', () => {
    const open = sidebar.classList.toggle('sidebar-open');
    toggle.setAttribute('aria-expanded', String(open));
  });

  document.querySelectorAll('.toast-close').forEach(button => {
    button.addEventListener('click', () => button.closest('.toast')?.remove());
  });
  document.querySelectorAll('.toast').forEach(toast => {
    window.setTimeout(() => toast.classList.add('toast-dismiss'), 6000);
    window.setTimeout(() => toast.remove(), 6500);
  });
})();
