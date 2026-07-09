import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

interface NavItem {
  iconClass: string;
  label: string;
  route: string;
  adminOnly?: boolean;
}

const ALL_NAV_ITEMS: NavItem[] = [
  { iconClass: 'fa-solid fa-house',       label: 'Dashboard',            route: '/dashboard'    },
  { iconClass: 'fa-solid fa-folder-open', label: 'Expedientes',          route: '/expedientes'  },
  { iconClass: 'fa-solid fa-building',    label: 'Comunidades',          route: '/comunidades'  },
  { iconClass: 'fa-solid fa-users',       label: 'Usuarios',             route: '/usuarios',    adminOnly: true },
  { iconClass: 'fa-solid fa-database',    label: 'Copias de Seguridad',  route: '/backup',      adminOnly: true },
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  private auth = inject(AuthService);

  readonly navItems = computed(() =>
    ALL_NAV_ITEMS.filter(item => !item.adminOnly || this.auth.isAdmin())
  );
}
